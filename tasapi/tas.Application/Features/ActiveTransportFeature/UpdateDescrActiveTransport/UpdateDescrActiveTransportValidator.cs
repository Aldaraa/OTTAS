using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.UpdateDescrActiveTransport;

namespace tas.Application.Features.ActiveTransporteFeature.UpdateDescrActiveTransporte
{
    public sealed class UpdateDescrActiveTransportValidator : AbstractValidator<UpdateDescrActiveTransportRequest>
    {
        public UpdateDescrActiveTransportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.description).NotEmpty().MaximumLength(50);



        }


    }
}
