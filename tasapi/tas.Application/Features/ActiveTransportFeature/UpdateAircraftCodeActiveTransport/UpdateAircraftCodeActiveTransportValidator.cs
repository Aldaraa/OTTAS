using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.UpdateAircraftCodeActiveTransport;

namespace tas.Application.Features.ActiveTransporteFeature.UpdateAircraftCodeActiveTransport
{
    public sealed class UpdateAircraftCodeActiveTransportValidator : AbstractValidator<UpdateAircraftCodeActiveTransportRequest>
    {
        public UpdateAircraftCodeActiveTransportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.AircraftCode).NotEmpty().MaximumLength(50);



        }


    }
}
