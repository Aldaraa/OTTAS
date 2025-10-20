using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus
{
    public sealed class GetEmployeeStatusValidator : AbstractValidator<GetEmployeeStatusRequest>
    {
        public GetEmployeeStatusValidator()
        {
            RuleFor(x => x.EventDate).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();
        }
    }


}
