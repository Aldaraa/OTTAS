using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.CreateRequestDeMobilisationType
{
    public sealed class CreateRequestDeMobilisationTypeMapper : Profile
    {
        public CreateRequestDeMobilisationTypeMapper()
        {
            CreateMap<CreateRequestDeMobilisationTypeRequest, RequestDeMobilisationType>();
        }
    }
}
