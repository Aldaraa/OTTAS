using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecute;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExecutePreview
{
    public sealed class BulkRosterExecutePreviewValidator : AbstractValidator<BulkRosterExecutePreviewRequest>
    {
        public BulkRosterExecutePreviewValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.DurationMonth).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Employees).NotEmpty();
            RuleForEach(x => x.Employees).SetValidator(new BulkRosterExecutePreviewEmployeesValidator());

        }
    }

    public class BulkRosterExecutePreviewEmployeesValidator : AbstractValidator<BulkRosterExecutePreviewEmployee>
    {
        public BulkRosterExecutePreviewEmployeesValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.RosterId).GreaterThan(0);
        }
    }
}
