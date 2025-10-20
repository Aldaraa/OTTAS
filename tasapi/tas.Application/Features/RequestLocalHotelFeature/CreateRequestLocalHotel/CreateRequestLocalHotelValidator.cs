using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLocalHotelFeature.CreateRequestLocalHotel
{
    public sealed class CreateRequestLocalHotelValidator : AbstractValidator<CreateRequestLocalHotelRequest>
    {
        public CreateRequestLocalHotelValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
