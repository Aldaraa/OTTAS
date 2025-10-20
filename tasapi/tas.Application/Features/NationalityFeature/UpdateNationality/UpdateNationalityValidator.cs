using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NationalityFeature.UpdateNationality;

namespace tas.Application.Features.NationalityeFeature.UpdateNationalitye
{
    public sealed class UpdateNationalityValidator : AbstractValidator<UpdateNationalityRequest>
    {
        public UpdateNationalityValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
