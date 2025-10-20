using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.SearchShortEmployee
{
    public sealed class SearchShortEmployeeMapper : Profile
    {
        public SearchShortEmployeeMapper()
        {
            CreateMap<Employee, SearchShortEmployeeResponse>();
        }
    }

}

