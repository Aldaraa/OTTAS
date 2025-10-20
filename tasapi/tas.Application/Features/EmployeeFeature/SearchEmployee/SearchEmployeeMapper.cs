using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.SearchEmployee
{
    public sealed class SearchEmployeeMapper : Profile
    {
        public SearchEmployeeMapper()
        {
            CreateMap<Employee, SearchEmployeeResponse>();
        }
    }

}

