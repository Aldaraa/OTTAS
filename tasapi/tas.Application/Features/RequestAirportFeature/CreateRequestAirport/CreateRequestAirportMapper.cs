using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestAirportFeature.CreateRequestAirport
{
    public sealed class CreateRequestAirportMapper : Profile
    {
        public CreateRequestAirportMapper()
        {
            CreateMap<CreateRequestAirportRequest, RequestAirport>();
        }
    }
}
