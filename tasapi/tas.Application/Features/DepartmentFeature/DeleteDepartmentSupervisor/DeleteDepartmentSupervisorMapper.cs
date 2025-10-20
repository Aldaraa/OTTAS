using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartmentSupervisor
{
    public sealed class DeleteDepartmentSupervisorMapper : Profile
    {
        public DeleteDepartmentSupervisorMapper()
        {
            CreateMap<DeleteDepartmentSupervisorRequest, DepartmentSupervisor>();
        }
    }
}
