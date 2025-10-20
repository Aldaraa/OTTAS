using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;

namespace tas.Application.Features.RequestGroupEmployeeFeature.SetPrimaryRequestGroupEmployees
{ 

    public sealed class SetPrimaryRequestGroupEmployeesValidator : AbstractValidator<SetPrimaryRequestGroupEmployeesRequest>
    {
        public SetPrimaryRequestGroupEmployeesValidator()
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
        }
    }
}
