using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.CreateDepartment
{
    public sealed class CreateDepartmentMapper : Profile
    {
        public CreateDepartmentMapper()
        {
            CreateMap<CreateDepartmentRequest, Department>();
        }
    }
}
