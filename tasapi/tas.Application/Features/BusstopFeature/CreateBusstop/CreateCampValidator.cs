using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BusstopFeature.CreateBusstop
{
    public sealed class CreateBusstopValidator : AbstractValidator<CreateBusstopRequest>
    {
        public CreateBusstopValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        }
    }
}
