using FluentValidation;
using EventStore.Models;

namespace EventStore.Core.Validators
{
    public class AddSnapshotValidator : AbstractValidator<AddSnapshot>
    {
        public AddSnapshotValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
