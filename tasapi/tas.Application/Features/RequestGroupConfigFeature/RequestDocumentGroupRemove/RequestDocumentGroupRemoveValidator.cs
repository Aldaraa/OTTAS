using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove
{
    public sealed class CreateRequestGroupRemoveValidator : AbstractValidator<RequestDocumentGroupRemoveRequest>
    {
        public CreateRequestGroupRemoveValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
