using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation
{
    public sealed class SearchEmployeeAccommodationMapper : Profile
    {
        public SearchEmployeeAccommodationMapper()
        {
            CreateMap<Employee, SearchEmployeeAccommodationResponse>();
        }
    }

}

