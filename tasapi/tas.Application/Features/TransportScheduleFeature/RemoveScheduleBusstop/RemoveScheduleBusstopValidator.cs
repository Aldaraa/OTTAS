using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.RemoveScheduleBusstop
{
    public sealed class RemoveScheduleBusstopValidator : AbstractValidator<RemoveScheduleBusstopRequest>
    {
        public RemoveScheduleBusstopValidator()
        {
            RuleFor(x => x.Id)
                       .NotEmpty().WithMessage("Id is required.");



        }




    }


}
