using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartmentAdmin
{
    public sealed class DeleteDepartmentAdminMapper : Profile
    {
        public DeleteDepartmentAdminMapper()
        {
            CreateMap<DeleteDepartmentAdminRequest, DepartmentAdmin>();
        }
    }
}
