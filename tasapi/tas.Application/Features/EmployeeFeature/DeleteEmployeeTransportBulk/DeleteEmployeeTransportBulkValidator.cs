using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployeeTransportBulk
{
    public sealed class DeleteEmployeeTransportBulkValidator : AbstractValidator<DeleteEmployeeTransportBulkRequest>
    {
        public DeleteEmployeeTransportBulkValidator()
        {
            RuleFor(request => request.employeeIds)
                      .NotEmpty().WithMessage("Employee IDs list cannot be empty.")
                      .Must(ids => ids.All(id => id > 0)).WithMessage("All employee IDs must be positive integers.");

            RuleFor(request => request.OnsiteDate).NotEmpty();

        }
    }
}
