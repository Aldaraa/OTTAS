using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.CreateRequestNonSiteTravelFlight 
{ 
    public sealed class CreateRequestNonSiteTravelFlightValidator : AbstractValidator<CreateRequestNonSiteTravelFlightRequest>
    {
        public CreateRequestNonSiteTravelFlightValidator()
        {
            //RuleFor(request => request.TravelDate).NotEmpty().WithMessage("TravelDate is required.");
            //RuleFor(request => request.ApproxTime).NotEmpty().WithMessage("ApproxTime is required.");
            //RuleFor(request => request.ETD).NotEmpty().WithMessage("ETD is required.");
            //RuleFor(request => request.DepartLocationId).NotEmpty().WithMessage("DepartLocationId is required.");
            //RuleFor(request => request.ArriveLocationId).NotEmpty().WithMessage("ArriveLocationId is required.");
            //RuleFor(request => request.DocumentId).NotEmpty().WithMessage("DocumentId is required.");

            RuleFor(request => request.TravelDate)
           .NotEmpty()
           .WithMessage(context => GenerateErrorMessage("TravelDate is required."));

            RuleFor(request => request.FavorTime)
                .NotEmpty()
                .WithMessage(context => GenerateErrorMessage("FavorTime is required."));

            RuleFor(request => request.DepartLocationId)
                .NotEmpty()
                .WithMessage(context => GenerateErrorMessage("DepartLocationId is required."));

            RuleFor(request => request.ArriveLocationId)
                .NotEmpty()
                .WithMessage(context => GenerateErrorMessage("ArriveLocationId is required."));

            RuleFor(request => request.DocumentId)
                .NotEmpty()
                .WithMessage(context => GenerateErrorMessage("DocumentId is required."));
        }

        private string GenerateErrorMessage(string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Error: ");
            builder.Append(message);
            // Add more logic to customize the error message if needed
            return builder.ToString();
        }
    }
}
