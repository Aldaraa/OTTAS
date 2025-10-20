using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CarrierFeature.UpdateCarrier;

namespace tas.Application.Features.CarriereFeature.UpdateCarriere
{
    public sealed class UpdateCarrierValidator : AbstractValidator<UpdateCarrierRequest>
    {
        public UpdateCarrierValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
