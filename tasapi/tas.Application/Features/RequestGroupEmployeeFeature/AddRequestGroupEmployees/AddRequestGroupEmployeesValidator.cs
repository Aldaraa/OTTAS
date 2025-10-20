using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;

namespace tas.Application.Features.RequestGroupEmployeeFeature.AddRequestGroupEmployees
{ 

    public sealed class AddRequestGroupEmployeesValidator : AbstractValidator<AddRequestGroupEmployeesRequest>
    {
        public AddRequestGroupEmployeesValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.GroupId).NotEmpty().GreaterThan(0);
        }
    }
}
