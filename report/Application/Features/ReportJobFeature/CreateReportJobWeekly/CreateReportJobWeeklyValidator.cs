using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateReportJobWeekly
{

    public sealed class CreateReportJobWeeklyValidator : AbstractValidator<CreateReportJobWeeklyRequest>
    {
        public CreateReportJobWeeklyValidator()
        {
            RuleFor(x => x.subscriptionMails)
                      .Must(emails => emails != null && emails.Count > 0)
                      .WithMessage("The list of email addresses must contain at least one element.")
                      .Must(ValidateEmails).WithMessage("Invalid email addresses found.");
            RuleFor(x => x.reportTemplateId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.startDate).NotEmpty();
            RuleFor(x=> x.recureEvery).NotEmpty();
            RuleFor(x => x.days)
                  .NotEmpty().WithMessage("The list of weekdays cannot be empty.")
                  .Must(ValidateWeekdays).WithMessage("Invalid weekdays found.");

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

        private bool ValidateWeekdays(List<string> days)
        {
            var validWeekdays = new HashSet<string> { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
            return days != null && days.All(m => validWeekdays.Contains(m));
        }
    }
}
