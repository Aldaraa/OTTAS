using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartment
{
    public sealed class GetAllDepartmentMapper : Profile
    {
        public GetAllDepartmentMapper()
        {
            CreateMap<Department, GetAllDepartmentResponse>();
        }
    }

}

