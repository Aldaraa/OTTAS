using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDeMobilisationTypeFeature.UpdateRequestDeMobilisationType;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.DeleteRequestDeMobilisationType
{

    public sealed class DeleteRequestDeMobilisationTypeMapper : Profile
    {
        public DeleteRequestDeMobilisationTypeMapper()
        {
            CreateMap<DeleteRequestDeMobilisationTypeRequest, RequestDeMobilisationType>();
        }
    }
}
