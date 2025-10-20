using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfile
{
    public sealed class ReplaceProfileValidator : AbstractValidator<ReplaceProfileRequest>
    {
        public ReplaceProfileValidator()
        {
            RuleFor(request => request.newEmployeeId).NotEmpty();
            RuleFor(request => request.oldEmployeeId).NotEmpty();

            RuleFor(request => request.EventId).GreaterThan(0).WithMessage("EventId must be greater than 0");
        }

    }
}
