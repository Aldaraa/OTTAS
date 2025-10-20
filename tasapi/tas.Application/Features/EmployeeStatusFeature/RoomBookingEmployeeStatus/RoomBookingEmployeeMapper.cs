using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee
{
    public sealed class GetAllEmployerMapper : Profile
    {
        public GetAllEmployerMapper()
        {
            CreateMap<EmployeeStatus, RoomBookingEmployeeResponse>();
        }
    }

}

