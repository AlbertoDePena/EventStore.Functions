using FluentValidation;
using EventStore.Core.Commands;

namespace EventStore.Core.Validators
{
    public class NewEventCommandValidator : AbstractValidator<NewEventCommand>
    {
        public NewEventCommandValidator()
        {
            RuleFor(x => x.Type).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
