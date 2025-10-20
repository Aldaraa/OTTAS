using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.UpdateRequestNonSiteTravelFlight 
{ 
    public sealed class UpdateRequestNonSiteTravelFlightValidator : AbstractValidator<UpdateRequestNonSiteTravelFlightRequest>
    {
        public UpdateRequestNonSiteTravelFlightValidator()
        {
            RuleFor(request => request.TravelDate).NotEmpty().WithMessage("TravelDate is required.");
            RuleFor(request => request.FavorTime).NotEmpty().WithMessage("FavorTime is required.");
            RuleFor(request => request.DepartLocationId).NotEmpty().WithMessage("DepartLocationId is required.");
            RuleFor(request => request.ArriveLocationId).NotEmpty().WithMessage("ArriveLocationId is required.");
            RuleFor(request => request.DocumentId).NotEmpty().WithMessage("DocumentId is required.");
            RuleFor(request => request.Id).NotEmpty().WithMessage("Id is required.");

        }
    }
}
