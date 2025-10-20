using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.LocationFeature.UpdateLocation;

namespace tas.Application.Features.LocationeFeature.UpdateLocatione
{
    public sealed class UpdateLocationValidator : AbstractValidator<UpdateLocationRequest>
    {
        public UpdateLocationValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Active).Must(value => value == 0 || value == 1);
        }
    }
}
