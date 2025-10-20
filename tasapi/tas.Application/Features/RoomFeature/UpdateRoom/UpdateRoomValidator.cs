using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.UpdateRoom;

namespace tas.Application.Features.RoomeFeature.UpdateRoome
{
    public sealed class UpdateRoomValidator : AbstractValidator<UpdateRoomRequest>
    {
        public UpdateRoomValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Number).NotEmpty().MaximumLength(50);
            RuleFor(x => x.CampId).NotEmpty();
            RuleFor(x => x.BedCount).NotEmpty();
            RuleFor(x => x.RoomTypeId).NotEmpty();
            RuleFor(x => x.Private).Must(value => value == 0 || value == 1);
        }
    }
}
