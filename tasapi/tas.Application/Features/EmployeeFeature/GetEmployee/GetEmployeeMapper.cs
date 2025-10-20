using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.GetEmployee
{
    public sealed class GetEmployeeMapper : Profile
    {
        public GetEmployeeMapper()
        {
            CreateMap<Employee, GetEmployeeResponse>();
        }
    }

}

