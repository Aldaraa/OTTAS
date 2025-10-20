using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;

namespace tas.Application.Features.RequestGroupEmployeeFeature.UpdateRequestGroupEmployees
{ 

    public sealed class UpdateRequestGroupEmployeesValidator : AbstractValidator<UpdateRequestGroupEmployeesRequest>
    {
        public UpdateRequestGroupEmployeesValidator()
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
        }
    }
}
