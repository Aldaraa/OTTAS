using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.UpdateVisitEvent
{
    public sealed class UpdateVisitEventValidator : AbstractValidator<UpdateVisitEventRequest>
    {
        public UpdateVisitEventValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.InScheduleId).NotEmpty();
            RuleFor(x => x.OutScheduleId).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();


        }
    }
}
