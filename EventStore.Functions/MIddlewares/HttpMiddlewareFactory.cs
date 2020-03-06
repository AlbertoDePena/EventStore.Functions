using System;
using Microsoft.Extensions.DependencyInjection;
using Numaka.Functions.Infrastructure;

namespace EventStore.Functions.Middlewares
{
    public class HttpMiddlewareFactory : IHttpMiddlewareFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public HttpMiddlewareFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public THandler Create<THandler>() where THandler : HttpMiddleware => _serviceProvider.GetService<THandler>();
    }
}