using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.UpdateShift;

namespace tas.Application.Features.ShifteFeature.UpdateShifte
{
    public sealed class UpdateShiftValidator : AbstractValidator<UpdateShiftRequest>
    {
        public UpdateShiftValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
