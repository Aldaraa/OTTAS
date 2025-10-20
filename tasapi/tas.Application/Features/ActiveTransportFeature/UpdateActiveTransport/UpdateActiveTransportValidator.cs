using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport;

namespace tas.Application.Features.ActiveTransporteFeature.UpdateActiveTransporte
{
    public sealed class UpdateActiveTransportValidator : AbstractValidator<UpdateActiveTransportRequest>
    {
        public UpdateActiveTransportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(m => m.Seats).GreaterThan(0);
            RuleFor(m => m.CarrierId).GreaterThan(0);
            RuleFor(m => m.StartDate).NotEmpty();
            RuleFor(m => m.EndDate).NotEmpty();



            RuleFor(request => request.ETD)
                .Must(BeValidDateTime)
                .When(request => !string.IsNullOrEmpty(request.ETD))
                .WithMessage("Invalid ETD value");

            RuleFor(request => request.ETA)
                .Must(BeValidDateTime)
                .When(request => !string.IsNullOrEmpty(request.ETA))
                .WithMessage("Invalid ETA value");
        }


        private bool BeValidDateTime(string value)
        {
            return string.IsNullOrEmpty(value) || (value.Length == 4 && int.TryParse(value, out _));
        }
    }
}
