using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BedFeature.DeleteBed
{
    public sealed class DeleteBedValidator : AbstractValidator<DeleteBedRequest>
    {
        public DeleteBedValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
