using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.ExtendActiveTransport
{
    public sealed class ExtendActiveTransportValidator : AbstractValidator<ExtendActiveTransportRequest>
    {
        public ExtendActiveTransportValidator()
        {
            RuleFor(x => x.ActiveTransportId).NotEmpty();
            RuleFor(x => x.Seats).NotEmpty();
            RuleFor(x => x.ETA)
                .NotEmpty()
                .Matches(@"^([01][0-9]|2[0-3])[0-5][0-9]$")
                .WithMessage("ETA must be in HHMM format.");

            RuleFor(x => x)
                .Must(x => IsETDLessThanETA(x.ETD, x.ETA))
                .WithMessage("ETD (departure time) must be earlier than ETA (arrival time).");



            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();


        }


        private bool IsETDLessThanETA(string etd, string eta)
        {
            if (TimeSpan.TryParseExact(etd, "hhmm", null, out var etdTime) &&
                TimeSpan.TryParseExact(eta, "hhmm", null, out var etaTime))
            {
                return etdTime < etaTime;
            }
            return false; // Return false if the time parsing fails
        }
    }
}
