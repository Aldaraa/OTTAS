using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CarrierFeature.DeleteCarrier
{
    public sealed class DeleteCarrierValidator : AbstractValidator<DeleteCarrierRequest>
    {
        public DeleteCarrierValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
