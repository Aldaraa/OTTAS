using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestAirportFeature.DeleteRequestAirport
{
    public sealed class DeleteRequestAirportValidator : AbstractValidator<DeleteRequestAirportRequest>
    {
        public DeleteRequestAirportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
