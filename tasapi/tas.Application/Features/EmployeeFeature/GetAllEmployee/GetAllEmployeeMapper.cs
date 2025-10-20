using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.GetAllEmployee
{
    public sealed class GetAllEmployeeMapper : Profile
    {
        public GetAllEmployeeMapper()
        {
            CreateMap<Employee, GetAllEmployeeResponse>();
        }
    }

}

