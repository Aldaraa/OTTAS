using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.RemoveExternalTransport
{
    public sealed class RemoveExternalTransportValidator : AbstractValidator<RemoveExternalTransportRequest>
    {
        public RemoveExternalTransportValidator()
        {
            RuleFor(x => x.TransportId).NotEmpty();

        }
    }
}
