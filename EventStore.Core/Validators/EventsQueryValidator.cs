using FluentValidation;
using EventStore.Core.Queries;

namespace EventStore.Core.Validators
{
    public class EventsQueryValidator : AbstractValidator<EventsQuery>
    {
        public EventsQueryValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.StartAtVersion).GreaterThanOrEqualTo(0);
        }
    }
}
