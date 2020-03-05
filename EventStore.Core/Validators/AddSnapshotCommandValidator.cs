using FluentValidation;
using EventStore.Core.Commands;

namespace EventStore.Core.Validators
{
    public class AddSnapshotCommandValidator : AbstractValidator<AddSnapshotCommand>
    {
        public AddSnapshotCommandValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
