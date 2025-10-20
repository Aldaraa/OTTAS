using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.AddExternalTravel;

namespace tas.Application.Features.TransportFeature.TransportBookingInfo
{
    public sealed class TransportBookingInfoValidator : AbstractValidator<TransportBookingInfoRequest>
    {
        public TransportBookingInfoValidator()
        {
            RuleFor(x => x.startDate)
                            .NotNull();

            RuleFor(x => x.endDate).NotEmpty(); 

        }
    }
}
