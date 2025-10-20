using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateTransportSchedule
{
    public sealed class UpdateTransportScheduleValidator : AbstractValidator<UpdateTransportScheduleRequest>
    {
        public UpdateTransportScheduleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TransportCode).NotEmpty().MaximumLength(50);
            RuleFor(m => m.Seats).GreaterThan(0);
            RuleFor(m => m.ETD).Length(4);
            RuleFor(m => m.ETA).Length(4);
            RuleFor(m => m.CarrierId).NotEmpty();
            RuleFor(m => m.TransportModeId).NotEmpty();


        }
    }
}
