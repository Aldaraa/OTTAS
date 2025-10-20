using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.ReScheduleMultiple;

namespace tas.Application.Features.TransportFeature.CreateNoGoShow
{

    public sealed class CreateNoGoShowValidator : AbstractValidator<CreateNoGoShowRequest>
    {
        public CreateNoGoShowValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.EventDate).NotEmpty();
            RuleFor(x => x.Direction).NotEmpty();

        }
    }
}
