using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChangeTemp
{


    public class UpdateRequestDocumentProfileChangeTempValidator : AbstractValidator<UpdateRequestDocumentProfileChangeTempRequest>
    {
        public UpdateRequestDocumentProfileChangeTempValidator()
        {
            RuleFor(x => x.Id).NotNull();

            RuleFor(x => x.EmployerId != null ||  x.CostCodeId != null || x.DepartmentId != null || x.PositionId != null
                ).NotEmpty().WithMessage("At least one of Employer, CostCode, Department, or Position must be provided.");

            When(x => x.Permanent == null || x.Permanent == 0, () =>
            {
                RuleFor(x => x.StartDate).NotNull().WithMessage("Start date is required.");
                RuleFor(x => x.EndDate).NotNull().WithMessage("End date is required.");
            });

            When(x => x.Permanent == 1, () =>
            {
                RuleFor(x => x.StartDate).NotNull().WithMessage("Start date is required.");
            });
        }
    }





}
