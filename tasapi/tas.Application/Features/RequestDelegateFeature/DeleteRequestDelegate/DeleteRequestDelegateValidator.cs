using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDelegateFeature.DeleteRequestDelegate
{
    public sealed class DeleteRequestDelegateValidator : AbstractValidator<DeleteRequestDelegateRequest>
    {
        public DeleteRequestDelegateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
