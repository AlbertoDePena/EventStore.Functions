using Numaka.Functions.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using EventStore.Functions.Handlers;

namespace EventStore.Functions
{
    public class Functions
    {
        private readonly IHttpFunctionContextBootstrapper _bootstrapper;
        private readonly IMiddlewarePipeline _pipeline;
        private readonly IHandlerFactory _factory;

        public Functions(IHttpFunctionContextBootstrapper bootstrapper, IMiddlewarePipeline pipeline, IHandlerFactory factory)
        {
            _bootstrapper = bootstrapper ?? throw new ArgumentNullException(nameof(bootstrapper));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [FunctionName("Streams")]
        public Task<HttpResponseMessage> GetStreams(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<StreamsHandler>(request, logger);

        [FunctionName("Events")]
        public Task<HttpResponseMessage> GetEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<EventsHandler>(request, logger);

        [FunctionName("Snapshots")]
        public Task<HttpResponseMessage> GetSnapshots(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS")] HttpRequestMessage request, ILogger logger)
            => ExecuteAsync<SnapshotsHandler>(request, logger);

        private async Task<HttpResponseMessage> ExecuteAsync<THttpMiddleware>(HttpRequestMessage request, ILogger logger) where THttpMiddleware : HttpMiddleware
        {
            logger.LogInformation("Bootstrapping HTTP function context...");

            var context = _bootstrapper.Bootstrap(request, logger);

            logger.LogInformation("Executing request...");

            // Order of middleware matters!!!
            _pipeline.Register(_factory.Create<CorsMiddleware>());
            _pipeline.Register(_factory.Create<SecurityMiddleware>());
            _pipeline.Register(_factory.Create<THttpMiddleware>());

            return await _pipeline.ExecuteAsync(context);
        }
    }
}