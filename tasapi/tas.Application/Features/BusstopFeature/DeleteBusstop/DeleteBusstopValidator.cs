using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BusstopFeature.DeleteBusstop
{
    public sealed class DeleteBusstopValidator : AbstractValidator<DeleteBusstopRequest>
    {
        public DeleteBusstopValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
