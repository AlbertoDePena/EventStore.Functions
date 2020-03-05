using FluentValidation;
using EventStore.Core.Queries;

namespace EventStore.Core.Validators
{
    public class StreamQueryValidator : AbstractValidator<StreamQuery>
    {
        public StreamQueryValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty();
        }
    }
}
