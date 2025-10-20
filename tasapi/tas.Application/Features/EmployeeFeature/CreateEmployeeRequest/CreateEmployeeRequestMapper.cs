using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.CreateEmployeeRequest
{
    public sealed class CreateEmployeeRequestMapper : Profile
    {
        public CreateEmployeeRequestMapper()
        {
            CreateMap<CreateEmployeeRequestRequest, Employee>();
        }
    }
}
