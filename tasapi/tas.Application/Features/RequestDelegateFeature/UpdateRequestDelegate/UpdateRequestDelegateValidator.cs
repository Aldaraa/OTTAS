using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDelegateFeature.UpdateRequestDelegate;

namespace tas.Application.Features.RequestDelegateeFeature.UpdateRequestDelegatee
{
    public sealed class UpdateRequestDelegateValidator : AbstractValidator<UpdateRequestDelegateRequest>
    {
        public UpdateRequestDelegateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.toEmployeeId).NotEmpty();
            RuleFor(x => x.toEmployeeId).NotEmpty();
            RuleFor(x => x.startDate).NotEmpty();
            RuleFor(x => x.endDate).NotEmpty();
            RuleFor(x => x)
                .Must(request => request.fromEmployeeId != request.toEmployeeId)
                .WithMessage("fromEmployee  and toEmployee cannot be the same.");
        }
    }
}
