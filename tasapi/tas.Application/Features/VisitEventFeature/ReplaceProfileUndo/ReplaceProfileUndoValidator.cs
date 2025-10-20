using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileUndo
{
    public sealed class ReplaceProfileUndoValidator : AbstractValidator<ReplaceProfileUndoRequest>
    {
        public ReplaceProfileUndoValidator()
        {
            RuleFor(request => request.EmployeeId).NotEmpty();
            RuleFor(request => request.EventId).GreaterThan(0).WithMessage("EventId must be greater than 0");
        }

    }
}
