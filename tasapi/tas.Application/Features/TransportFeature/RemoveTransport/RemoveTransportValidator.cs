using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.RemoveTransport
{
    public sealed class RemoveTransportValidator : AbstractValidator<RemoveTransportRequest>
    {
        public RemoveTransportValidator()
        {
            RuleFor(c => c.endScheduleId).NotEmpty();
            RuleFor(c => c.startScheduleId).NotEmpty();
            RuleFor(c => c.shiftId).NotEmpty();


        }
    }
}
