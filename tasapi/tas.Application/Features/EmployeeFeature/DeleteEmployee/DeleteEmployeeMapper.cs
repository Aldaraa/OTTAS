using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.DeleteEmployee
{

    public sealed class DeleteEmployeeMapper : Profile
    {
        public DeleteEmployeeMapper()
        {
            CreateMap<DeleteEmployeeRequest, Employee>();
        }
    }
}
