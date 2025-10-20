using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestTravelPurposeFeature.DeleteRequestTravelPurpose
{
    public sealed class DeleteRequestTravelPurposeValidator : AbstractValidator<DeleteRequestTravelPurposeRequest>
    {
        public DeleteRequestTravelPurposeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
