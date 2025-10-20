using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateReportJobTimeValidate
{

    public sealed class CreateReportJobTimeValidateValidator : AbstractValidator<CreateReportJobTimeValidateRequest>
    {
        public CreateReportJobTimeValidateValidator()
        {
            RuleFor(x => x.scheduleStartDate).NotEmpty();
        }


    }
}
