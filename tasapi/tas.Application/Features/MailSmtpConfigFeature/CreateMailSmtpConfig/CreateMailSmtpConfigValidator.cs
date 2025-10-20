using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MailSmtpConfigFeature.CreateMailSmtpConfig
{
    public sealed class CreateMailSmtpConfigValidator : AbstractValidator<CreateMailSmtpConfigRequest>
    {
        public CreateMailSmtpConfigValidator()
        {
            RuleFor(x => x.smtpPort).NotEmpty();
            RuleFor(x => x.smtpServer).NotEmpty();

        }
    }
}
