using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestLocalHotelFeature.UpdateRequestLocalHotel;

namespace tas.Application.Features.RequestLocalHoteleFeature.UpdateRequestLocalHotele
{
    public sealed class UpdateRequestLocalHotelValidator : AbstractValidator<UpdateRequestLocalHotelRequest>
    {
        public UpdateRequestLocalHotelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
