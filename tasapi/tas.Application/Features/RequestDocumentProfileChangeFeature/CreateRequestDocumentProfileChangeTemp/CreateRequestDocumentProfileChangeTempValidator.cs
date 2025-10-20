using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChangeTemp
{


    public class CreateRequestDocumentProfileChangeTempValidator : AbstractValidator<CreateRequestDocumentProfileChangeTempRequest>
    {
        public CreateRequestDocumentProfileChangeTempValidator()
        {
            
            RuleFor(x => x.Employee).NotNull();
            RuleFor(x => x.changeRequestData.action).NotEmpty();

            RuleFor(x => x.Employee)
                .Must(employee =>
                    employee.EmployerId != null ||
                    employee.CostCodeId != null ||
                    employee.DepartmentId != null ||
                    employee.PositionId != null
                )
                .WithMessage("At least one of Employer, CostCode, Department, or Position must be provided.");

            When(x => x.Employee.Permanent == null || x.Employee.Permanent == 0, () =>
            {
                RuleFor(x => x.Employee.StartDate).NotNull().WithMessage("Start date is required.");
                RuleFor(x => x.Employee.EndDate).NotNull().WithMessage("End date is required.");
            });

            When(x => x.Employee.Permanent == 1, () =>
            {
                RuleFor(x => x.Employee.StartDate).NotNull().WithMessage("Start date is required.");
            });
        }
    }
   
}
