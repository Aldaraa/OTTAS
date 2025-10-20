using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.CreateRequestDeMobilisationType
{
    public sealed class CreateRequestDeMobilisationTypeValidator : AbstractValidator<CreateRequestDeMobilisationTypeRequest>
    {
        public CreateRequestDeMobilisationTypeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
