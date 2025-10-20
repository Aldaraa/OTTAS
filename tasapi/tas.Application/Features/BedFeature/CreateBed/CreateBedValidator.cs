using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BedFeature.CreateBed
{
    public sealed class CreateBedValidator : AbstractValidator<CreateBedRequest>
    {
        public CreateBedValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(20);
            RuleFor(x => x.RoomId).NotEmpty();

        }
    }
}
