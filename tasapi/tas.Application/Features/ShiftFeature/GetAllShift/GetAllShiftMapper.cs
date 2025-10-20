using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.ShiftFeature.GetAllShift
{
    public sealed class GetAllShiftMapper : Profile
    {
        public GetAllShiftMapper()
        {
            CreateMap<Shift, GetAllShiftResponse>();
        }
    }

}

