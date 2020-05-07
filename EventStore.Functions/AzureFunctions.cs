using Numaka.Functions.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using EventStore.Functions.Middlewares;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;
using System.Collections.Generic;
using EventStore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Aliencube.AzureFunctions.Extensions.OpenApi;
using Aliencube.AzureFunctions.Extensions.OpenApi.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Aliencube.AzureFunctions.Extensions.OpenApi.Extensions;
using Microsoft.OpenApi;
using System.Reflection;

namespace EventStore.Functions
{
    public class AzureFunctions
    {
        private readonly IServiceProvider _serviceProvider;

        public AzureFunctions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        [FunctionName(nameof(GetSnapshots))]
        [OpenApiOperation("get-snapshots", "EventStore")]
        [OpenApiParameter(name: "streamName", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Snapshot>), Description = "Get snapshots for the given stream")]
        public Task<IActionResult> GetSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<GetSnapshotsHandler>(request, logger);

        [FunctionName(nameof(GetEvents))]
        [OpenApiOperation("get-events", "EventStore")]
        [OpenApiParameter(name: "streamName", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter(name: "startAtVersion", In = ParameterLocation.Query, Required = false, Type = typeof(int))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Event>))]
        public Task<IActionResult> GetEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<GetEventsHandler>(request, logger);

        [FunctionName(nameof(FindStream))]
        [OpenApiOperation("find-stream", "EventStore")]
        [OpenApiParameter(name: "streamName", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(Stream))]
        public Task<IActionResult> FindStream(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<FindStreamHandler>(request, logger);

        [FunctionName(nameof(GetAllStreams))]
        [OpenApiOperation("get-all-streams", "EventStore")]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Stream>))]
        public Task<IActionResult> GetAllStreams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<GetAllStreamsHandler>(request, logger);

        [FunctionName(nameof(AppendEvents))]
        [OpenApiOperation("append-events", "EventStore")]
        [OpenApiRequestBody("application/json", typeof(AppendEvents))]
        [OpenApiResponseBody(HttpStatusCode.NoContent, "application/json", typeof(string))]
        public Task<IActionResult> AppendEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<AppendEventsHandler>(request, logger);

        [FunctionName(nameof(AddSnapshot))]
        [OpenApiOperation("add-snapshots", "EventStore")]
        [OpenApiRequestBody("application/json", typeof(AddSnapshot))]
        [OpenApiResponseBody(HttpStatusCode.NoContent, "application/json", typeof(string))]
        public Task<IActionResult> AddSnapshot(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<AddSnapshotHandler>(request, logger);

        [FunctionName(nameof(DeleteSnapshots))]
        [OpenApiOperation("delete-snapshots", "EventStore")]
        [OpenApiParameter(name: "streamName", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseBody(HttpStatusCode.NoContent, "application/json", typeof(string))]
        public Task<IActionResult> DeleteSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", "options")] HttpRequest request, ILogger logger)
            => ExecuteAsync<DeleteSnapshotsHandler>(request, logger);

        [FunctionName(nameof(RenderSwaggerDocument))]
        [OpenApiIgnore]
        public async Task<IActionResult> RenderSwaggerDocument(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/{version}.{format}")] HttpRequest request, string version, string format, ILogger logger)
        {
            logger.LogInformation("Configuring swagger document...");

            var openApiSettings = _serviceProvider.GetService<OpenApiAppSettings>();

            var filter = new RouteConstraintFilter();
            var helper = new DocumentHelper(filter);
            var document = new Document(helper);

            var result = await document.InitialiseDocument()
                                       .AddMetadata(openApiSettings.OpenApiInfo)
                                       .AddServer(request, routePrefix: "api")
                                       .Build(Assembly.GetExecutingAssembly(), new CamelCaseNamingStrategy())
                                       .RenderAsync(GetVersion(version), GetFormat(format));

            return new ContentResult()
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [FunctionName(nameof(RenderSwaggerUI))]
        [OpenApiIgnore]
        public async Task<IActionResult> RenderSwaggerUI(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/ui/{version}.{format}")] HttpRequest request, string version, string format, ILogger logger)
        {
            logger.LogInformation("Configuring swagger UI...");

            var openApiSettings = _serviceProvider.GetService<OpenApiAppSettings>();

            var ui = new SwaggerUI();
            var result = await ui.AddMetadata(openApiSettings.OpenApiInfo)
                                 .AddServer(request, routePrefix: "api")
                                 .BuildAsync()
                                 .RenderAsync($"swagger/{version}.{format}", openApiSettings.SwaggerAuthKey);

            return new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        private async Task<IActionResult> ExecuteAsync<TMiddleware>(HttpRequest request, ILogger logger) where TMiddleware : HttpMiddleware
        {
            var bootstrapper = _serviceProvider.GetService<IHttpFunctionContextBootstrapper>();
            var pipeline = _serviceProvider.GetService<IMiddlewarePipeline>();

            logger.LogInformation("Bootstrapping HTTP function context...");

            var context = bootstrapper.Bootstrap(request, logger);

            logger.LogInformation("Registering middlewares...");

            // Order of middleware matters!!!
            pipeline.Register(_serviceProvider.GetService<CorsMiddleware>());
            pipeline.Register(_serviceProvider.GetService<ExceptionMiddleware>());
            //pipeline.Register(_serviceProvider.GetService<SecurityMiddleware>());
            pipeline.Register(_serviceProvider.GetService<TMiddleware>());

            logger.LogInformation("Executing request...");

            return await pipeline.ExecuteAsync(context);
        }

        private OpenApiSpecVersion GetVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (version.Equals("v2", StringComparison.CurrentCultureIgnoreCase))
            {
                return OpenApiSpecVersion.OpenApi2_0;
            }

            if (version.Equals("v3", StringComparison.CurrentCultureIgnoreCase))
            {
                return OpenApiSpecVersion.OpenApi3_0;
            }

            throw new InvalidOperationException("Invalid Open API version");
        }

        private OpenApiFormat GetFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentNullException(nameof(format));
            }

            return Enum.TryParse(format, true, out OpenApiFormat result)
                       ? result
                       : throw new InvalidOperationException("Invalid Open API format");
        }
    }
}
