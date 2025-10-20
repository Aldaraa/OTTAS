using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.PeopleTypeFeature.DeletePeopleType
{
    public sealed class DeletePeopleTypeValidator : AbstractValidator<DeletePeopleTypeRequest>
    {
        public DeletePeopleTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
