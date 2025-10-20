using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobMonthly
{

    public sealed class UpdateReportJobMonthlyValidator : AbstractValidator<UpdateReportJobMonthlyRequest>
    {
        public UpdateReportJobMonthlyValidator()
        {
            RuleFor(x => x.subscriptionMails)
                      .Must(emails => emails != null && emails.Count > 0)
                      .WithMessage("The list of email addresses must contain at least one element.")
                      .Must(ValidateEmails).WithMessage("Invalid email addresses found.");
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.reportTemplateId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.startDate).NotEmpty();
            RuleFor(x => x.months)
                  .NotEmpty().WithMessage("The list of months cannot be empty.")
                  .Must(ValidateMonths).WithMessage("Invalid months found.");
            RuleFor(x => x.days).NotEmpty();
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

        private bool ValidateMonths(List<string> months)
        {
            var validMonths = new HashSet<string> { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            return months != null && months.All(m => validMonths.Contains(m));
        }

    }
}
