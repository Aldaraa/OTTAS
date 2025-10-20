using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupDetailFeature.CreateGroupDetail
{
    public sealed class CreateGroupDetailValidator : AbstractValidator<CreateGroupDetailRequest>
    {
        public CreateGroupDetailValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(1).MaximumLength(50);
            RuleFor(x => x.GroupMasterId).NotEmpty();
        }
    }
}
