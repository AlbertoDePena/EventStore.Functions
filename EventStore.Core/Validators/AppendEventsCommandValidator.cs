using FluentValidation;
using EventStore.Core.Commands;
using System;

namespace EventStore.Core.Validators
{
    public class AppendEventsCommandValidator : AbstractValidator<AppendEventsCommand>
    {
        public AppendEventsCommandValidator(IValidator<NewEventCommand> newEventValidator)
        {
            if (newEventValidator == null) throw new ArgumentNullException(nameof(newEventValidator));

            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.ExpectedVersion).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Events).Must(x => x.Length > 0).WithMessage("Please provide some events");
            RuleForEach(x => x.Events).SetValidator(newEventValidator);
        }
    }
}
