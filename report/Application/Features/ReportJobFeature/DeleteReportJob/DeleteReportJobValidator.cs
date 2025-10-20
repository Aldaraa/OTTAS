using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.DeleteReportJob
{

    public sealed class DeleteReportJobValidator : AbstractValidator<DeleteReportJobRequest>
    {
        public DeleteReportJobValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }


    }
}
