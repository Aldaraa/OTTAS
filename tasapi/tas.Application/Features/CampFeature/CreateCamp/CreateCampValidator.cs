using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CampFeature.CreateCamp
{
    public sealed class CreateCampValidator : AbstractValidator<CreateCampRequest>
    {
        public CreateCampValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
        }
    }
}
