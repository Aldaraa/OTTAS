using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.UpdateShift;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeTransportFeature.UpdateTransport
{
    public sealed class UpdateTransportValidator : AbstractValidator<UpdateTransportRequest>
    {
        public UpdateTransportValidator()
        {
            RuleFor(x => x.ScheduleId).NotEmpty();
            RuleFor(x => x.TransportId).NotEmpty();
        }
    }


}
