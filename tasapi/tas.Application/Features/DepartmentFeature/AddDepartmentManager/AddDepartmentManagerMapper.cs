using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.AddDepartmentManager
{
    public sealed class AddDepartmentManagerMapper : Profile
    {
        public AddDepartmentManagerMapper()
        {
            CreateMap<AddDepartmentManagerRequest, DepartmentManager>();
        }
    }
}
