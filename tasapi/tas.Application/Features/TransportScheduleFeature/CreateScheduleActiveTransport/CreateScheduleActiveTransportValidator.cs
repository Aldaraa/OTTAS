using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.CreateScheduleActiveTransport
{
    public sealed class CreateScheduleActiveTransportRequestValidator : AbstractValidator<CreateScheduleActiveTransportRequest>
    {
        public CreateScheduleActiveTransportRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
          //  RuleFor(m => m.inSeats).GreaterThan(0);
            RuleFor(m => m.TransportModeId).GreaterThan(0);
            RuleFor(m => m.fromLocationId).GreaterThan(0);
            RuleFor(m => m.toLocationId).GreaterThan(0);
            RuleFor(m => m.CarrierId).GreaterThan(0);
            RuleFor(m => m.FrequencyWeeks).GreaterThan(0);


            RuleFor(x => x.StartDate)
           .NotEmpty().WithMessage("Start Date is required.");


            RuleFor(x => x.EndDate)
           .NotEmpty().WithMessage("End date is required.")
           .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.");

            RuleFor(x => x.dayNums).Must(BeAValidDayOfWeek)
          .WithMessage("Invalid day of week. Please provide a valid day of week.");

            RuleFor(x => x)
    .Must(x => IsETDLessThanETA(x.ETD, x.ETA))
    .WithMessage("ETD must be less than ETA.");

            When(x => !string.IsNullOrEmpty(x.outETA) && !string.IsNullOrEmpty(x.outETD), () =>
            {
                RuleFor(x => x)
                    .Must(x => IsETDLessThanETA(x.outETD, x.outETA))
                    .WithMessage("OUTETD must be less than OUTETA.");
            });



        }

        private bool BeAValidDayOfWeek(string[] dayNums)
        {
            return dayNums.All(day => new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }.Contains(day));
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
