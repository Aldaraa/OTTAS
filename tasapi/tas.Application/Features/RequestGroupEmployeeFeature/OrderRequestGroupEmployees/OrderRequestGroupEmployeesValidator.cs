using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;

namespace tas.Application.Features.RequestGroupEmployeeFeature.OrderRequestGroupEmployees
{ 

    public sealed class OrderRequestGroupEmployeesValidator : AbstractValidator<OrderRequestGroupEmployeesRequest>
    {
        public OrderRequestGroupEmployeesValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().GreaterThan(0);
            RuleForEach(x => x.Ids).GreaterThan(0).WithMessage("Ids must be greater than 0.");
        }
    }
}
