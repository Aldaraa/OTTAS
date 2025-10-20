using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestAirportFeature.UpdateRequestAirport;

namespace tas.Application.Features.RequestAirporteFeature.UpdateRequestAirporte
{
    public sealed class UpdateRequestAirportValidator : AbstractValidator<UpdateRequestAirportRequest>
    {
        public UpdateRequestAirportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(50);

        }
    }
}
