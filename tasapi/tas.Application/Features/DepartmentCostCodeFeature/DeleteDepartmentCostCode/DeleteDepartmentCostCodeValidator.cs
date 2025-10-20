using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode
{
    public sealed class DeleteDepartmentCostCodeValidator : AbstractValidator<DeleteDepartmentCostCodeRequest>
    {
        public DeleteDepartmentCostCodeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
