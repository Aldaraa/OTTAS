using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportModeFeature.DeleteTransportMode
{
    public sealed class DeleteTransportModeValidator : AbstractValidator<DeleteTransportModeRequest>
    {
        public DeleteTransportModeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
