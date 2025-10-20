using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.CreateShift;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeTransportFeature.CreateTransport
{
    public sealed class CreateTransportValidator : AbstractValidator<CreateTransportRequest>
    {
        public CreateTransportValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.CostCodeId).NotEmpty();
            RuleFor(x => x.EmployerId).NotEmpty();
        }
    }


}
