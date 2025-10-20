using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployeeTransport
{
    public sealed class DeleteEmployeeTransportValidator : AbstractValidator<DeleteEmployeeTransportRequest>
    {
        public DeleteEmployeeTransportValidator()
        {
            RuleFor(x => x.employeeId).NotEmpty();
            RuleFor(x => x.OnsiteDate).NotEmpty();

        }
    }
}
