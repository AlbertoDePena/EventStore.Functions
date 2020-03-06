using FluentValidation;
using System;
using System.Linq;
using EventStore.Models;

namespace EventStore.Core.Validators
{
    public class AppendEventsValidator : AbstractValidator<AppendEvents>
    {
        public AppendEventsValidator(IValidator<NewEvent> newEventValidator)
        {
            if (newEventValidator == null) throw new ArgumentNullException(nameof(newEventValidator));

            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.ExpectedVersion).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Events).Must(x => x.Any()).WithMessage("Please provide some events");
            RuleForEach(x => x.Events).SetValidator(newEventValidator);
        }
    }
}
