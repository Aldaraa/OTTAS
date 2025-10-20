using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeDataGroup
{
    public sealed class CreateCostCodeValidator : AbstractValidator<ChangeEmployeeDataGroupRequest>
    {
        public CreateCostCodeValidator()
        {

            RuleFor(x => x.GroupMasterId).NotEmpty();
            RuleFor(x => x.GroupMasterId).NotEmpty();

        }

    }
}
