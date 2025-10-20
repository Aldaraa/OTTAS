using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomTypeFeature.CreateRoomType
{
    public sealed class CreateRoomTypeValidator : AbstractValidator<CreateRoomTypeRequest>
    {
        public CreateRoomTypeValidator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        }
    }
}
