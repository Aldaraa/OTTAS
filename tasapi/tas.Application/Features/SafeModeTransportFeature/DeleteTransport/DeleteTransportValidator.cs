using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.DeleteShift;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeTransportFeature.DeleteTransport
{
    public sealed class DeleteTransportValidator : AbstractValidator<DeleteTransportRequest>
    {
        public DeleteTransportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }


}
