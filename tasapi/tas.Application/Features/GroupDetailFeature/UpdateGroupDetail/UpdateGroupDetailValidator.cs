using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.CreateGroupDetail;

namespace tas.Application.Features.GroupDetailFeature.UpdateGroupDetail
{
    public sealed class UpdateGroupDetailValidator : AbstractValidator<UpdateGroupDetailRequest>
    {
        public UpdateGroupDetailValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.GroupMasterId).NotEmpty();
        }
    }
}
