using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport
{
    public sealed class DeleteScheduleValidator : AbstractValidator<DeleteActiveTransportRequest>
    {
        public DeleteScheduleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
