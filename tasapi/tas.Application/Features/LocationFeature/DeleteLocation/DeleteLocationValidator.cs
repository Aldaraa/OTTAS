using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.LocationFeature.DeleteLocation
{
    public sealed class DeleteLocationValidator : AbstractValidator<DeleteLocationRequest>
    {
        public DeleteLocationValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
