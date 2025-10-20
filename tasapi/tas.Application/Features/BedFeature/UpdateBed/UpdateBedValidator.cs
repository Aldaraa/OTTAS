using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.UpdateBed;

namespace tas.Application.Features.BedeFeature.UpdateBede
{
    public sealed class UpdateBedValidator : AbstractValidator<UpdateBedRequest>
    {
        public UpdateBedValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(20);
            RuleFor(x => x.RoomId).NotEmpty();

        }
    }
}
