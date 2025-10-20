using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployerFeature.UpdateEmployer;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.DeleteEmployer
{

    public sealed class DeleteEmployerMapper : Profile
    {
        public DeleteEmployerMapper()
        {
            CreateMap<DeleteEmployerRequest, Employer>();
        }
    }
}
