using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.DeleteRequestNonSiteTravelAccommodation 
{ 
    public sealed class DeleteRequestNonSiteTravelAccommodationValidator : AbstractValidator<DeleteRequestNonSiteTravelAccommodationRequest>
    {
        public DeleteRequestNonSiteTravelAccommodationValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
