using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.DeleteRosterDetail
{
    public sealed class DeleteRosterDetailValidator : AbstractValidator<DeleteRosterDetailRequest>
    {
        public DeleteRosterDetailValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
