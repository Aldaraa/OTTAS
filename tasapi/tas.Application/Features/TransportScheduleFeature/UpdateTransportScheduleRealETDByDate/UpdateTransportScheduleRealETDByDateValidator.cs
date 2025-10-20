using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETDByDate
{
    public sealed class UpdateTransportScheduleRealETDByDateValidator : AbstractValidator<UpdateTransportScheduleRealETDByDateRequest>
    {
        public UpdateTransportScheduleRealETDByDateValidator()
        {
            RuleFor(x => x.ActiveTransportId).NotEmpty();
            RuleFor(x => x.ScheduleDate).NotEmpty();
            RuleFor(m => m.RealETD)
                      .NotEmpty().WithMessage("Real ETD is required.")
                      .Length(4).WithMessage("Real ETD must be exactly 4 characters long.")
                      .Matches(@"^([01][0-9]|2[0-3])[0-5][0-9]$")
                      .WithMessage("Real ETD must be in the format HHmm and represent a valid time.");


        }
    }
}
