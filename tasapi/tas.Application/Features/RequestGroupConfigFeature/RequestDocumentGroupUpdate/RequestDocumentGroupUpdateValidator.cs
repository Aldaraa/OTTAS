using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate
{
    public sealed class CreateRequestGroupValidator : AbstractValidator<RequestDocumentGroupUpdateRequest>
    {
        public CreateRequestGroupValidator()
        {
            RuleFor(x => x.document).NotEmpty().MaximumLength(100);
            RuleFor(x => x.groupId).NotEmpty();
        }
    }
}
