using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PeopleTypeFeature.UpdatePeopleType;

namespace tas.Application.Features.PeopleTypeeFeature.UpdatePeopleTypee
{
    public sealed class UpdatePeopleTypeValidator : AbstractValidator<UpdatePeopleTypeRequest>
    {
        public UpdatePeopleTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
