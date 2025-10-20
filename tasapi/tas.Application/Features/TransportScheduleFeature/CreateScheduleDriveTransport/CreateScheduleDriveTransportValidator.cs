using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.CreateScheduleDriveTransport
{
    public sealed class CreateScheduleDriveTransportRequestValidator : AbstractValidator<CreateScheduleDriveTransportRequest>
    {
        public CreateScheduleDriveTransportRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
          //  RuleFor(m => m.inSeats).GreaterThan(0);
            RuleFor(m => m.fromLocationId).GreaterThan(0);
            RuleFor(m => m.toLocationId).GreaterThan(0);
            

            RuleFor(x => x.StartDate)
           .NotEmpty().WithMessage("Start Date is required.");


            RuleFor(x => x.EndDate)
           .NotEmpty().WithMessage("End date is required.")
           .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.");

            RuleFor(x => x.dayNums).Must(BeAValidDayOfWeek)
          .WithMessage("Invalid day of week. Please provide a valid day of week.");



        }

        private bool BeAValidDayOfWeek(string[] dayNums)
        {
            return dayNums.All(day => new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }.Contains(day));
        }

    }
}
