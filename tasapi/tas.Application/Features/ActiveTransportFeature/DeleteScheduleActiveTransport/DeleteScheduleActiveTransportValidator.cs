using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.DeleteScheduleActiveTransport
{
    public sealed class DeleteScheduleScheduleValidator : AbstractValidator<DeleteScheduleActiveTransportRequest>
    {
        public DeleteScheduleScheduleValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
