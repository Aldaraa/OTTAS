using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.UpdateRequestNonSiteTravelAccommodation 
{ 
    public sealed class UpdateRequestNonSiteTravelAccommodationValidator : AbstractValidator<UpdateRequestNonSiteTravelAccommodationRequest>
    {
        public UpdateRequestNonSiteTravelAccommodationValidator()
        {
            RuleFor(request => request.PaymentCondition).NotEmpty().WithMessage("PaymentCondition is required.");
            RuleFor(request => request.Hotel).NotEmpty().WithMessage("Hotel is required.");
            RuleFor(request => request.FirstNight).NotEmpty().WithMessage("FirstNight is required.");
            RuleFor(request => request.LastNight).NotEmpty().WithMessage("LastNight is required.");
            RuleFor(request => request.DocumentId).NotEmpty().WithMessage("DocumentId is required.");
            RuleFor(request => request.Id).NotEmpty().WithMessage("Id is required.");

        }
    }
}
