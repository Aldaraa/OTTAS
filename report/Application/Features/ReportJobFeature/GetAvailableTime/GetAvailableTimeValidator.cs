using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetAvailableTime
{

    public sealed class CreateReportJobTimeValidator : AbstractValidator<GetAvailableTimeRequest>
    {
        public CreateReportJobTimeValidator()
        {
            RuleFor(x => x.scheduleStartDate).NotEmpty();
        }


    }
}
