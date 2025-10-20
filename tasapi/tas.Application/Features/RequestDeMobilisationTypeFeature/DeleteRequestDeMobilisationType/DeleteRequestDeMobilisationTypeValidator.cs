using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.DeleteRequestDeMobilisationType
{
    public sealed class DeleteRequestDeMobilisationTypeValidator : AbstractValidator<DeleteRequestDeMobilisationTypeRequest>
    {
        public DeleteRequestDeMobilisationTypeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
