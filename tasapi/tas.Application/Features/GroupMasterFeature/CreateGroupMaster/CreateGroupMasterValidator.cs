using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.CreateGroupMaster
{
    public sealed class CreateGroupMasterValidator : AbstractValidator<CreateGroupMasterRequest>
    {
        public CreateGroupMasterValidator()
        {
            RuleFor(x => x.description).NotEmpty();
            RuleFor(x => x.ShowOnProfile).Must(value => value == 0 || value == 1);

        }
    }
}
