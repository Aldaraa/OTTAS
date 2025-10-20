using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.UpdateBusstopActiveTransport;

namespace tas.Application.Features.ActiveTransporteFeature.UpdateBusstopActiveTransporte
{
    public sealed class UpdateBusstopActiveTransportValidator : AbstractValidator<UpdateBusstopActiveTransportRequest>
    {
        public UpdateBusstopActiveTransportValidator()
        {
            RuleFor(x => x.Id)
                       .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.startDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThanOrEqualTo(x => x.endDate)
                .WithMessage("Start date must be earlier than or equal to the end date.");

            RuleFor(x => x.endDate)
                .NotEmpty().WithMessage("End date is required.");

            RuleForEach(x => x.Busstops).SetValidator(new UpdateBusstopActiveTransportRequestBussttopsValidator());



        }


        public sealed class UpdateBusstopActiveTransportRequestBussttopsValidator : AbstractValidator<UpdateBusstopActiveTransportRequestBussttops>
        {
            public UpdateBusstopActiveTransportRequestBussttopsValidator()
            {
                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Bus stop description is required.");

                RuleFor(x => x.ETD)
                    .NotEmpty().WithMessage("ETD is required.")
                    .Length(4).WithMessage("ETD must be exactly 4 characters long.")
                    .Matches(@"^([01][0-9]|2[0-3])[0-5][0-9]$")
                    .WithMessage("ETD must be in the format HHmm and represent a valid time.");
            }
        }

    }
}
