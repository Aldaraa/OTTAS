using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLocalHotelFeature.DeleteRequestLocalHotel
{
    public sealed class DeleteRequestLocalHotelValidator : AbstractValidator<DeleteRequestLocalHotelRequest>
    {
        public DeleteRequestLocalHotelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
