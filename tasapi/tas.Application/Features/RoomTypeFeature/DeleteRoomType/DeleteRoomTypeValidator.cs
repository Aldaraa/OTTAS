using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomTypeFeature.DeleteRoomType
{
    public sealed class DeleteRoomTypeValidator : AbstractValidator<DeleteRoomTypeRequest>
    {
        public DeleteRoomTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
