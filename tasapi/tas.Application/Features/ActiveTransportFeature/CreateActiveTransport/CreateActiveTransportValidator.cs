using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.CreateActiveTransport
{
    public sealed class CreateActiveTransportValidator : AbstractValidator<CreateActiveTransportRequest>
    {
        public CreateActiveTransportValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
            RuleFor(m => m.DayNum)
                .Must(day => new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }.Contains(day))
                .WithMessage("The DayNum field must be one of 'Monday', 'Tuesday', or 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'.");
            RuleFor(m => m.Direction)
                .Must(day => new[] { "IN", "OUT"}.Contains(day))
                .WithMessage("The Direction field must be one of 'IN' or 'OUT'.");
            RuleFor(m => m.Seats).GreaterThan(0);
            RuleFor(m => m.TransportModeId).GreaterThan(0);
            RuleFor(m => m.fromLocationId).GreaterThan(0);
            RuleFor(m => m.toLocationId).GreaterThan(0);
            RuleFor(m => m.CarrierId).GreaterThan(0);
            RuleFor(x => x.TransportAudit)
             .Must(value => value == 0 || value == 1)
             .WithMessage("TransportAudit must be either 0 or 1.");
            RuleFor(x => x.Special)
             .Must(value => value == 0 || value == 1)
             .WithMessage("Special must be either 0 or 1.");
            RuleFor(m => m.FrequencyWeeks).GreaterThan(0);

        }
    }
}
