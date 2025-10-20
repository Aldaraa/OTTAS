using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.CreateVisitEvent
{
    public sealed class CreateVisitEventValidator : AbstractValidator<CreateVisitEventRequest>
    {
        public CreateVisitEventValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.startDate).NotEmpty();
            RuleFor(x => x.endDate).NotEmpty();
            RuleFor(x => x.HeadCount).GreaterThan(0);
        }
    }
}
