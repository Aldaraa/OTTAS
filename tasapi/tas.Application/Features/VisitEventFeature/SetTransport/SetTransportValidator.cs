using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.SetTransport
{
    public sealed class SetTransportValidator : AbstractValidator<SetTransportRequest>
    {
        public SetTransportValidator()
        {
            RuleFor(x => x.InScheduleId).NotEmpty().GreaterThan(0);
            RuleFor(x => x.OutScheduleId).NotEmpty().GreaterThan(0);
            RuleFor(x=> x.EventId).GreaterThan(0);
            RuleFor(x=>x.EmpIds).NotNull();
        }
    }
}
