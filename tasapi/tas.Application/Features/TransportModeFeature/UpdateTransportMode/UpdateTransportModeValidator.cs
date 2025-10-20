using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportModeFeature.UpdateTransportMode;

namespace tas.Application.Features.TransportModeeFeature.UpdateTransportModee
{
    public sealed class UpdateTransportModeValidator : AbstractValidator<UpdateTransportModeRequest>
    {
        public UpdateTransportModeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);

        }
    }
}
