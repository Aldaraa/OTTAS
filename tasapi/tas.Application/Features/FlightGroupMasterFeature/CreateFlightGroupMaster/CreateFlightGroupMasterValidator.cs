using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.CreateFlightGroupMaster
{
    public sealed class CreateFlightGroupMasterValidator : AbstractValidator<CreateFlightGroupMasterRequest>
    {
        public CreateFlightGroupMasterValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);

            RuleFor(x => x.DayPattern)
                .Must(dayPattern => new[] { 28, 42, 56 }.Contains(dayPattern))
                .WithMessage("DayPattern must be one of the following values: 28, 42, 56.");
        }
    }
}
