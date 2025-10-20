using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.StatusEmployee
{
    public sealed class StatusEmployeeMapper : Profile
    {
        public StatusEmployeeMapper()
        {
            CreateMap<Employee, StatusEmployeeResponse>();
        }
    }

}

