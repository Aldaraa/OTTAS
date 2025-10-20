using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CarrierFeature.CreateCarrier
{
    public sealed class CreateCarrierValidator : AbstractValidator<CreateCarrierRequest>
    {
        public CreateCarrierValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
