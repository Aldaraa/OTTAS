using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.DeleteFlightGroupMaster
{
    public sealed class DeleteFlightGroupMasterValidator : AbstractValidator<DeleteFlightGroupMasterRequest>
    {
        public DeleteFlightGroupMasterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
