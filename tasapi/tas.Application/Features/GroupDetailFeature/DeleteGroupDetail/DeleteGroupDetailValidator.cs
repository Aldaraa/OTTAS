using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.UpdateGroupDetail;

namespace tas.Application.Features.GroupDetailFeature.DeleteGroupDetail
{
    public sealed class DeleteGroupDetailValidator : AbstractValidator<DeleteGroupDetailRequest>
    {
        public DeleteGroupDetailValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
