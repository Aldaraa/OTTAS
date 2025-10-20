using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PeopleTypeFeature.CreatePeopleType
{
    public sealed class CreatePeopleTypeValidator : AbstractValidator<CreatePeopleTypeRequest>
    {
        public CreatePeopleTypeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).MaximumLength(150);
        }
    }
}
