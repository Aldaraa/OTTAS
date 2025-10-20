using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupOrder
{
    public sealed class CreateRequestGroupValidator : AbstractValidator<RequestDocumentGroupOrderRequest>
    {
        public CreateRequestGroupValidator()
        {
            RuleFor(x => x.document).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Ids).NotEmpty().WithMessage("Ids list cannot be empty.");
            RuleForEach(x => x.Ids).GreaterThan(0).WithMessage("Ids must be greater than 0.");
        }
    }
}
