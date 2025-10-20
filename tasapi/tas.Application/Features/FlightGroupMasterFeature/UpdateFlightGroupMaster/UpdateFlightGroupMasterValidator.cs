using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.FlightGroupMasterFeature.UpdateFlightGroupMaster;

namespace tas.Application.Features.FlightGroupMastereFeature.UpdateFlightGroupMastere
{
    public sealed class UpdateFlightGroupMasterValidator : AbstractValidator<UpdateFlightGroupMasterRequest>
    {
        public UpdateFlightGroupMasterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
