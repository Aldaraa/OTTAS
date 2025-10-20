using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentFeature.UpdateDepartment;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartment
{

    public sealed class DeleteDepartmentMapper : Profile
    {
        public DeleteDepartmentMapper()
        {
            CreateMap<DeleteDepartmentRequest, Department>();
        }
    }
}
