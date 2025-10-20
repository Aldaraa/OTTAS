using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelPurposeFeature.DeleteRequestTravelPurpose;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestPurposeAgentFeature.DeleteRequestTravelPurpose
{

    public sealed class DeleteRequestTravelPurposeMapper : Profile
    {
        public DeleteRequestTravelPurposeMapper()
        {
            CreateMap<DeleteRequestTravelPurposeRequest, RequestTravelPurpose>();
        }
    }
}
