using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CampFeature.UpdateCamp;

namespace tas.Application.Features.CampeFeature.UpdateCampe
{
    public sealed class UpdateCampValidator : AbstractValidator<UpdateCampRequest>
    {
        public UpdateCampValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
