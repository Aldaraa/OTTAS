using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AgreementFeature.CreateAgreement
{
    public sealed class CreateAgreementValidator : AbstractValidator<CreateAgreementRequest>
    {
        public CreateAgreementValidator()
        {
            RuleFor(x => x.AgreementText).NotEmpty();

        }
    }
}
