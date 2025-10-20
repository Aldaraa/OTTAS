using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.DeleteRequestGroup
{
    public sealed class DeleteRequestGroupValidator : AbstractValidator<DeleteRequestGroupRequest>
    {
        public DeleteRequestGroupValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
