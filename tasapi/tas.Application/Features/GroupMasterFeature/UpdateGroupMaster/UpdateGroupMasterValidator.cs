using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMasterFeature.UpdateGroupMaster;

namespace tas.Application.Features.GroupMastereFeature.UpdateGroupMastere
{
    public sealed class UpdateGroupMasterValidator : AbstractValidator<UpdateGroupMasterRequest>
    {
        public UpdateGroupMasterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description);
            RuleFor(x => x.ShowOnProfile).Must(value => value == 0 || value == 1);

        }
    }
}
