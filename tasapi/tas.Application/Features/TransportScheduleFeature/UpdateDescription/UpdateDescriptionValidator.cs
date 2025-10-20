using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateDescription
{
    public sealed class UpdateDescriptionValidator : AbstractValidator<UpdateDescriptionRequest>
    {
        public UpdateDescriptionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.description).NotEmpty().MaximumLength(50);



        }


    }
}
