using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate
{
    public sealed class CreateRequestDelegateValidator : AbstractValidator<CreateRequestDelegateRequest>
    {
        public CreateRequestDelegateValidator()
        {
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
