using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobRuntime
{

    public sealed class UpdateReportJobRuntimeValidator : AbstractValidator<UpdateReportJobRuntimeRequest>
    {
        public UpdateReportJobRuntimeValidator()
        {
            RuleFor(x => x.subscriptionMails)
                      .Must(emails => emails != null && emails.Count > 0)
                      .WithMessage("The list of email addresses must contain at least one element.")
                      .Must(ValidateEmails).WithMessage("Invalid email addresses found.");
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.reportTemplateId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.executeDate).NotEmpty();


        }


        private bool ValidateEmails(List<string> emails)
        {
            if (emails == null || !emails.Any())
            {
                return true;
            }
            string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return emails.All(email => Regex.IsMatch(email, emailPattern));
        }
    }
}
