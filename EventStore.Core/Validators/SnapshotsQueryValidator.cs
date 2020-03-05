using FluentValidation;
using EventStore.Core.Queries;

namespace EventStore.Core.Validators
{
    public class SnapshotsQueryValidator : AbstractValidator<SnapshotsQuery>
    {
        public SnapshotsQueryValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
        }
    }
}
