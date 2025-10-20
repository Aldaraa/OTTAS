using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.BuildReport
{

    public sealed class BuildReportValidator : AbstractValidator<BuildReportRequest>
    {
        public BuildReportValidator()
        {
            RuleFor(x => x.reportTemplateId).NotEmpty();
        }
    }
}
