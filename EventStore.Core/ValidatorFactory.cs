using FluentValidation;
using System;
using Microsoft.Extensions.DependencyInjection;


namespace EventStore.Core
{
    public class ValidatorFactory : Contracts.IValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IValidator<T> GetValidator<T>() => _serviceProvider.GetService<IValidator<T>>();
    }
}
