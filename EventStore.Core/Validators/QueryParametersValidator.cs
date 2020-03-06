using EventStore.Models;
using FluentValidation;

namespace EventStore.Core.Validators
{
    public class QueryParametersValidator : AbstractValidator<QueryParameters>
    {
        public QueryParametersValidator()
        {
            RuleFor(x => x.StreamName).NotEmpty().MaximumLength(256);
            RuleFor(x => x.StartAtVersion).GreaterThanOrEqualTo(0).When(x => x.StartAtVersion.HasValue);
        }
    }
}
