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
    public class SnapshotsFunction
    {
        private readonly IHttpFunctionContextBootstrapper _bootstrapper;
        private readonly IMiddlewarePipeline _pipeline;
        private readonly IHttpMiddlewareFactory _factory;

        public SnapshotsFunction(IHttpFunctionContextBootstrapper bootstrapper, IMiddlewarePipeline pipeline, IHttpMiddlewareFactory factory)
        {
            _bootstrapper = bootstrapper ?? throw new ArgumentNullException(nameof(bootstrapper));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [FunctionName("Snapshots")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS")] HttpRequestMessage request, ILogger logger)
        {
            logger.LogInformation("Bootstrapping HTTP function context...");

            var context = _bootstrapper.Bootstrap(request, logger);

            logger.LogInformation("Executing request...");

            // Order of middleware matters!!!
            _pipeline.Register(_factory.Create<CorsMiddleware>());
            _pipeline.Register(_factory.Create<SecurityMiddleware>());
            _pipeline.Register(_factory.Create<SnapshotsHandler>());

            return await _pipeline.ExecuteAsync(context);
        }
    }
}