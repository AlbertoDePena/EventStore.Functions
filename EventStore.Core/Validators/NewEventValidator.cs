using FluentValidation;
using EventStore.Models;

namespace EventStore.Core.Validators
{
    public class NewEventValidator : AbstractValidator<NewEvent>
    {
        public NewEventValidator()
        {
            RuleFor(x => x.Type).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
