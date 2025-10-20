using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.CreateRoom
{
    public sealed class CreateRoomValidator : AbstractValidator<CreateRoomRequest>
    {
        public CreateRoomValidator()
        {
            RuleFor(x => x.Number).NotEmpty().MaximumLength(50);
            RuleFor(x => x.CampId).NotEmpty();
            RuleFor(x => x.BedCount)
                .NotEmpty()
                .LessThanOrEqualTo(6).WithMessage("BedCount must be less than or equal to 6.");
            RuleFor(x => x.Private).Must(value => value == 0 || value == 1);
            RuleFor(x => x.RoomTypeId).NotEmpty();

        }
    }
}
