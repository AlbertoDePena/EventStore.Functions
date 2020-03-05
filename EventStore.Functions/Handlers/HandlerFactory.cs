using System;
using Microsoft.Extensions.DependencyInjection;
using Numaka.Functions.Infrastructure;

namespace EventStore.Functions.Handlers
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public THandler Create<THandler>() where THandler : HttpMiddleware => _serviceProvider.GetService<THandler>();
    }
}