using FluentValidation;

namespace EventStore.Core.Contracts
{
    public interface IValidatorFactory
    {
        IValidator<T> GetValidator<T>();
    }
}
