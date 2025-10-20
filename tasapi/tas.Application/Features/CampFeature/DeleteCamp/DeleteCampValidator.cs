using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.CampFeature.DeleteCamp
{
    public sealed class DeleteCampValidator : AbstractValidator<DeleteCampRequest>
    {
        public DeleteCampValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
