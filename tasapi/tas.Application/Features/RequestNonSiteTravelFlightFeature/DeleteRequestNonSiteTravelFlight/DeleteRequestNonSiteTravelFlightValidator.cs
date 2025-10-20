using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.DeleteRequestNonSiteTravelFlight 
{ 
    public sealed class DeleteRequestNonSiteTravelFlightValidator : AbstractValidator<DeleteRequestNonSiteTravelFlightRequest>
    {
        public DeleteRequestNonSiteTravelFlightValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
