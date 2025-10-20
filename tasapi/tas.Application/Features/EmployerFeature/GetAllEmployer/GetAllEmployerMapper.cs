using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.GetAllEmployer
{
    public sealed class GetAllEmployerMapper : Profile
    {
        public GetAllEmployerMapper()
        {
            CreateMap<Employer, GetAllEmployerResponse>();
        }
    }

}

