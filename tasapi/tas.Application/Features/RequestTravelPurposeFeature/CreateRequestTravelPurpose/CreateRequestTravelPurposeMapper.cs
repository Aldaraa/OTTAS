using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelPurposeFeature.CreateRequestTravelPurpose
{
    public sealed class CreateRequestTravelPurposeMapper : Profile
    {
        public CreateRequestTravelPurposeMapper()
        {
            CreateMap<CreateRequestTravelPurposeRequest, RequestTravelPurpose>();
        }
    }
}
