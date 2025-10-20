using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ShiftFeature.DeleteShift
{
    public sealed class DeleteShiftValidator : AbstractValidator<DeleteShiftRequest>
    {
        public DeleteShiftValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
