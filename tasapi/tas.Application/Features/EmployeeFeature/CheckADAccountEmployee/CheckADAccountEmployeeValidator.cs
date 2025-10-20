using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.CheckADAccountEmployee
{
    public sealed class RosterExecutePreviewEmployeeValidator : AbstractValidator<CheckADAccountEmployeeRequest>
    {
        public RosterExecutePreviewEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0).WithMessage("EmployeeId should be greater than 0.");
            RuleFor(x => x.AdAccount).NotEmpty().WithMessage("AdAccount cannot be empty.");
         

        }
    }
}
