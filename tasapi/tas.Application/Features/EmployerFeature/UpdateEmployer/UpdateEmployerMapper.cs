using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.UpdateEmployer
{

    public sealed class UpdateEmployerMapper : Profile
    {
        public UpdateEmployerMapper()
        {
            CreateMap<UpdateEmployerRequest, Employer>();
        }
    }
}
