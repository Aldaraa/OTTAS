using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateScheduleBusstop
{
    public sealed class UpdateScheduleBusstopValidator : AbstractValidator<UpdateScheduleBusstopRequest>
    {
        public UpdateScheduleBusstopValidator()
        {
            RuleFor(x => x.Id)
                       .NotEmpty().WithMessage("Id is required.");

            // Ensure Busstops list is not empty
            RuleFor(x => x.Busstops)
                .NotEmpty().WithMessage("At least one bus stop is required.");

            // Validate each bus stop in the list
            RuleForEach(x => x.Busstops).SetValidator(new UpdateScheduleBusstopRequestBusstopsValidator());


        }




    }


    public sealed class UpdateScheduleBusstopRequestBusstopsValidator : AbstractValidator<UpdateScheduleBusstopRequestBussttops>
    {
        public UpdateScheduleBusstopRequestBusstopsValidator()
        {
            // Validate Description is not empty
            RuleFor(x => x)
                .NotEmpty().WithMessage("Bus stop description is required.");

            // Validate ETD follows the HHmm format
            RuleFor(x => x.ETD)
                .NotEmpty().WithMessage("ETD is required.")
                .Length(4).WithMessage("ETD must be exactly 4 characters long.")
                .Matches(@"^([01][0-9]|2[0-3])[0-5][0-9]$")
                .WithMessage("ETD must be in the format HHmm and represent a valid time.");
        }
    }
}
