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
    public class AppendEventsFunction
    {
        private readonly IServiceProvider _serviceProvider;

        public AppendEventsFunction(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        [FunctionName("AppendEvents")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", "OPTIONS", Route = "append-events")] HttpRequestMessage request, ILogger logger)
        {
            var bootstrapper = _serviceProvider.GetService<IHttpFunctionContextBootstrapper>();
            var pipeline = _serviceProvider.GetService<IMiddlewarePipeline>();

            logger.LogInformation("Bootstrapping HTTP function context...");

            var context = bootstrapper.Bootstrap(request, logger);

            logger.LogInformation("Registering middlewares...");

            // Order of middleware matters!!!
            pipeline.Register(_serviceProvider.GetService<CorsMiddleware>());
            pipeline.Register(_serviceProvider.GetService<SecurityMiddleware>());
            pipeline.Register(_serviceProvider.GetService<AppendEventsMiddleware>());

            logger.LogInformation("Executing request...");

            return await pipeline.ExecuteAsync(context);
        }
    }
}
