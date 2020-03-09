using Numaka.Functions.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventStore.Functions.Middlewares;

namespace EventStore.Functions
{
    public class AzureFunctions
    {
        private readonly IServiceProvider _serviceProvider;

        public AzureFunctions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        [FunctionName("GetSnapshots")]
        public Task<HttpResponseMessage> GetSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS", Route = "get-snapshots")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<GetSnapshotsMiddleware>(request, logger);

        [FunctionName("GetEvents")]
        public Task<HttpResponseMessage> GetEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS", Route = "get-events")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<GetEventsMiddleware>(request, logger);

        [FunctionName("FindStream")]
        public Task<HttpResponseMessage> FindStream(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS", Route = "find-stream")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<FindStreamMiddleware>(request, logger);

        [FunctionName("GetAllStreams")]
        public Task<HttpResponseMessage> GetAllStreams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS", Route = "get-all-streams")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<GetAllStreamsMiddleware>(request, logger);

        [FunctionName("AppendEvents")]
        public Task<HttpResponseMessage> AppendEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", "OPTIONS", Route = "append-events")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<AppendEventsMiddleware>(request, logger);

        [FunctionName("AddSnapshot")]
        public Task<HttpResponseMessage> AddSnapshot(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", "OPTIONS", Route = "add-snapshot")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<AddSnapshotMiddleware>(request, logger);

        [FunctionName("DeleteSnapshots")]
        public Task<HttpResponseMessage> DeleteSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", "OPTIONS", Route = "delete-snapshots")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<DeleteSnapshotsMiddleware>(request, logger);

        private async Task<HttpResponseMessage> ExecuteAsync<TMiddleware>(HttpRequestMessage request, ILogger logger) where TMiddleware : HttpMiddleware
        {
            var bootstrapper = _serviceProvider.GetService<IHttpFunctionContextBootstrapper>();
            var pipeline = _serviceProvider.GetService<IMiddlewarePipeline>();

            logger.LogInformation("Bootstrapping HTTP function context...");

            var context = bootstrapper.Bootstrap(request, logger);

            logger.LogInformation("Registering middlewares...");

            // Order of middleware matters!!!
            pipeline.Register(_serviceProvider.GetService<CorsMiddleware>());
            pipeline.Register(_serviceProvider.GetService<ExceptionMiddleware>());
            pipeline.Register(_serviceProvider.GetService<SecurityMiddleware>());
            pipeline.Register(_serviceProvider.GetService<TMiddleware>());

            logger.LogInformation("Executing request...");

            return await pipeline.ExecuteAsync(context);
        }
    }
}