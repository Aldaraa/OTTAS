using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDeMobilisationTypeFeature.UpdateRequestDeMobilisationType;

namespace tas.Application.Features.RequestDeMobilisationTypeeFeature.UpdateRequestDeMobilisationType
{
    public sealed class UpdateRequestDeMobilisationTypeValidator : AbstractValidator<UpdateRequestDeMobilisationTypeRequest>
    {
        public UpdateRequestDeMobilisationTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
