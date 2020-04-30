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

        [FunctionName(nameof(GetSnapshots))]
        public Task<HttpResponseMessage> GetSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<GetSnapshotsHandler>(request, logger);

        [FunctionName(nameof(GetEvents))]
        public Task<HttpResponseMessage> GetEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<GetEventsHandler>(request, logger);

        [FunctionName(nameof(FindStream))]
        public Task<HttpResponseMessage> FindStream(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<FindStreamHandler>(request, logger);

        [FunctionName(nameof(GetAllStreams))]
        public Task<HttpResponseMessage> GetAllStreams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<GetAllStreamsHandler>(request, logger);

        [FunctionName(nameof(AppendEvents))]
        public Task<HttpResponseMessage> AppendEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<AppendEventsHandler>(request, logger);

        [FunctionName(nameof(AddSnapshot))]
        public Task<HttpResponseMessage> AddSnapshot(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<AddSnapshotHandler>(request, logger);

        [FunctionName(nameof(DeleteSnapshots))]
        public Task<HttpResponseMessage> DeleteSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", "options")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<DeleteSnapshotsHandler>(request, logger);

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
            //pipeline.Register(_serviceProvider.GetService<SecurityMiddleware>());
            pipeline.Register(_serviceProvider.GetService<TMiddleware>());

            logger.LogInformation("Executing request...");

            return await pipeline.ExecuteAsync(context);
        }
    }
}