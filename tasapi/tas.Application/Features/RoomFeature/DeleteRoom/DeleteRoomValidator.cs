using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RoomFeature.DeleteRoom
{
    public sealed class DeleteRoomValidator : AbstractValidator<DeleteRoomRequest>
    {
        public DeleteRoomValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
