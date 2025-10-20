using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.ShiftFeature.CreateShift
{
    public sealed class CreateShiftMapper : Profile
    {
        public CreateShiftMapper()
        {
            CreateMap<CreateShiftRequest, Shift>();
        }
    }
}
