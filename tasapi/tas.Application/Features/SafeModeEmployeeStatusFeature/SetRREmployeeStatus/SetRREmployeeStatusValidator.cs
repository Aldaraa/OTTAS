using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.CreateShift;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.SetRREmployeeStatus
{
    public sealed class SetRREmployeeStatusValidator : AbstractValidator<SetRREmployeeStatusRequest>
    {
        public SetRREmployeeStatusValidator()
        {
            RuleFor(x => x.EventDate).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();
        }
    }


}
