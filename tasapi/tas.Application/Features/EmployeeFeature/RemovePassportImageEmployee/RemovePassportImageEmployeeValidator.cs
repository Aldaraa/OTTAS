using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.RemovePassportImageEmployee
{
    public sealed class RemovePassportImageEmployeeValidator : AbstractValidator<RemovePassportImageEmployeeRequest>
    {
        public RemovePassportImageEmployeeValidator()
        {
            RuleFor(x => x.employeeId).NotEmpty();

        }
    }
}
