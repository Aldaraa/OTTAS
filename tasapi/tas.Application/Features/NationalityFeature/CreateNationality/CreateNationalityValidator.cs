using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.NationalityFeature.CreateNationality
{
    public sealed class CreateNationalityValidator : AbstractValidator<CreateNationalityRequest>
    {
        public CreateNationalityValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
