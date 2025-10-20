using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail
{
    public sealed class SetClusterFlightGroupDetailMapper : Profile
    {
        public SetClusterFlightGroupDetailMapper()
        {
            CreateMap<SetClusterFlightGroupDetailRequest, FlightGroupDetail>();
        }
    }
}
