using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.CreateSpecialActiveTransport
{
    public sealed class CreateSpecialActiveTransportRequestValidator : AbstractValidator<CreateSpecialActiveTransportRequest>
    {
        public CreateSpecialActiveTransportRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(m => m.Seats).GreaterThan(0);

            RuleFor(m => m.TransportModeId).GreaterThan(0);
            RuleFor(m => m.fromLocationId).GreaterThan(0);
            RuleFor(m => m.toLocationId).GreaterThan(0);
            RuleFor(m => m.CarrierId).GreaterThan(0);
            RuleFor(x => x.EventDate).NotEmpty();
            RuleFor(m => m.CostCodeId).GreaterThan(0);


            RuleFor(x => x)
                .Must(x => IsETDLessThanETA(x.ETD, x.ETA))
                .WithMessage("ETD must be less than ETA.");

            When(x => !string.IsNullOrEmpty(x.OUTETA) && !string.IsNullOrEmpty(x.OUTETD), () =>
            {
                RuleFor(x => x)
                    .Must(x => IsETDLessThanETA(x.OUTETD, x.OUTETA))
                    .WithMessage("OUTETD must be less than OUTETA.");
            });
        }


        private bool IsETDLessThanETA(string etd, string eta)
        {
            if (DateTime.TryParseExact(etd, "HHmm", null, System.Globalization.DateTimeStyles.None, out DateTime etdTime) &&
                     DateTime.TryParseExact(eta, "HHmm", null, System.Globalization.DateTimeStyles.None, out DateTime etaTime))
            {
                return etdTime.TimeOfDay < etaTime.TimeOfDay;
            }
            return false;
        }

    }
}
