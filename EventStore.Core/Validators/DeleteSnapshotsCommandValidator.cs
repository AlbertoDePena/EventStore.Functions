using FluentValidation;
using EventStore.Core.Commands;

namespace EventStore.Core.Validators
{
    public class DeleteSnapshotsCommandValidator : AbstractValidator<DeleteSnapshotsCommand>
    {
        public DeleteSnapshotsCommandValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
        }
    }
}
