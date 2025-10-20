using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee
{
    public sealed class RosterExecutePreviewEmployeeValidator : AbstractValidator<RosterExecutePreviewEmployeeRequest>
    {
        public RosterExecutePreviewEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0).WithMessage("EmployeeId should be greater than 0.");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("StartDate cannot be empty.");
            RuleFor(x => x.MonthDuration).GreaterThan(0).WithMessage("MonthDuration should be greater than 0.");
            RuleFor(x => x.RosterId).GreaterThan(0).WithMessage("RosterId should be greater than 0.");
            RuleFor(x => x.FlightGroupMasterId).GreaterThan(0).WithMessage("FlightGroupMasterId should be greater than 0.");

        }
    }
}
