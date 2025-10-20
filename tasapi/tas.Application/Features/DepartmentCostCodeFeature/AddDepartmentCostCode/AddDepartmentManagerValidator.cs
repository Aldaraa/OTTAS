using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode
{
    public sealed class AddDepartmentCostCodeValidator : AbstractValidator<AddDepartmentCostCodeRequest>
    {
        public AddDepartmentCostCodeValidator()
        {
            RuleFor(x => x.CostCodeId).NotEmpty();
            RuleFor(x => x.DepartmentId).NotEmpty();

        }
    }
}
