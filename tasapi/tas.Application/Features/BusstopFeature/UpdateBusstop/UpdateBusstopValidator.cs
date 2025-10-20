using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BusstopFeature.UpdateBusstop;

namespace tas.Application.Features.BusstopeFeature.UpdateBusstope
{
    public sealed class UpdateBusstopValidator : AbstractValidator<UpdateBusstopRequest>
    {
        public UpdateBusstopValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
