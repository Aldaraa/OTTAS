using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecute;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExecute
{
    public sealed class BulkRosterExecuteValidator : AbstractValidator<BulkRosterExecuteRequest>
    {
        public BulkRosterExecuteValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.DurationMonth).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Employees).NotNull();
            RuleForEach(x => x.Employees).SetValidator(new BulkRosterExecuteEmployeesValidator());
        }

    }

    public class BulkRosterExecuteEmployeesValidator : AbstractValidator<BulkRosterExecuteEmployee>
    {
        public BulkRosterExecuteEmployeesValidator()
        {
            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.RosterId).GreaterThan(0);
        }
    }


}
