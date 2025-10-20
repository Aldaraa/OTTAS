using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.NationalityFeature.DeleteNationality
{
    public sealed class DeleteNationalityValidator : AbstractValidator<DeleteNationalityRequest>
    {
        public DeleteNationalityValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
