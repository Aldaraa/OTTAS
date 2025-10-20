using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.CreateRoom;

namespace tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees
{
    public sealed class RemoveRequestGroupEmployeesValidator : AbstractValidator<RemoveRequestGroupEmployeesRequest>
    {
        public RemoveRequestGroupEmployeesValidator()
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);

        }
    }
}
