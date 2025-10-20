using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.DeleteGroupMaster
{
    public sealed class DeleteGroupMasterValidator : AbstractValidator<DeleteGroupMasterRequest>
    {
        public DeleteGroupMasterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
