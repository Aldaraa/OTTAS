using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.ReScheduleMultiple;

namespace tas.Application.Features.TransportFeature.DeleteNoGoShow
{

    public sealed class DeleteNoGoShowValidator : AbstractValidator<DeleteNoGoShowRequest>
    {
        public DeleteNoGoShowValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

        }
    }
}
