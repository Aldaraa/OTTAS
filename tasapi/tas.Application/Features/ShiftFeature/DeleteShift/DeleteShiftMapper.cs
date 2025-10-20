using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.UpdateShift;
using tas.Domain.Entities;

namespace tas.Application.Features.ShiftFeature.DeleteShift
{

    public sealed class DeleteShiftMapper : Profile
    {
        public DeleteShiftMapper()
        {
            CreateMap<DeleteShiftRequest, Shift>();
        }
    }
}
