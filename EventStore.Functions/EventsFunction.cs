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
    public class EventsFunction
    {
        private readonly IServiceProvider _serviceProvider;

        public EventsFunction(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        [FunctionName("Events")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "OPTIONS")] HttpRequestMessage request, ILogger logger)
        {
            var bootstrapper = _serviceProvider.GetService<IHttpFunctionContextBootstrapper>();
            var pipeline = _serviceProvider.GetService<IMiddlewarePipeline>();

            logger.LogInformation("Bootstrapping HTTP function context...");

            var context = bootstrapper.Bootstrap(request, logger);

            logger.LogInformation("Registering middlewares...");

            // Order of middleware matters!!!
            pipeline.Register(_serviceProvider.GetService<CorsMiddleware>());
            pipeline.Register(_serviceProvider.GetService<SecurityMiddleware>());
            pipeline.Register(_serviceProvider.GetService<EventsMiddleware>());

            logger.LogInformation("Executing request...");

            return await pipeline.ExecuteAsync(context);
        }
    }
}