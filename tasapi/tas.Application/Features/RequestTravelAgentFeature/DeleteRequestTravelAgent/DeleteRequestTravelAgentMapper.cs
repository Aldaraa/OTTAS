using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelAgentFeature.DeleteRequestTravelAgent
{

    public sealed class DeleteRequestTravelAgentMapper : Profile
    {
        public DeleteRequestTravelAgentMapper()
        {
            CreateMap<DeleteRequestTravelAgentRequest, RequestTravelAgent>();
        }
    }
}
