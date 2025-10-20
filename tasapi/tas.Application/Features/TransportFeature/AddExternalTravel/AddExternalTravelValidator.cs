using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.AddExternalTravel
{
    public sealed class AddExternalTravelValidator : AbstractValidator<AddExternalTravelRequest>
    {
        public AddExternalTravelValidator()
        {
            RuleFor(x => x.EmployeeId)
                            .NotNull().WithMessage("Employee ID is required.")
                            .GreaterThan(0).WithMessage("Employee ID must be a positive number greater than zero.");

            RuleFor(x => x.FirstSheduleId)
                .NotNull().WithMessage("Schedule ID is required.")
                .GreaterThan(0).WithMessage("From Schedule ID must be a positive number greater than zero.");


            RuleFor(x => x.DepartmentId)
                .NotNull().WithMessage("Department ID is required.")
                .GreaterThan(0).WithMessage("Department ID must be a positive number greater than zero.");

            RuleFor(x => x.PositionId)
                .NotNull().WithMessage("Position ID is required.")
                .GreaterThan(0).WithMessage("Position ID must be a positive number greater than zero.");

            RuleFor(x => x.EmployerId)
                .NotNull().WithMessage("Employer ID is required.")
                .GreaterThan(0).WithMessage("Employer ID must be a positive number greater than zero.");

            RuleFor(x => x.CostCodeId)
                .NotNull().WithMessage("Cost Code ID is required.")
                .GreaterThan(0).WithMessage("Cost Code ID must be a positive number greater than zero.");
        }
    }
}
