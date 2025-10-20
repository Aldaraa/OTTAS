using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomTypeFeature.UpdateRoomType;

namespace tas.Application.Features.RoomTypeeFeature.UpdateRoomTypee
{
    public sealed class UpdateRoomTypeValidator : AbstractValidator<UpdateRoomTypeRequest>
    {
        public UpdateRoomTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
