using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.UpdateFlightGroupMaster
{

    public sealed class UpdateFlightGroupMasterMapper : Profile
    {
        public UpdateFlightGroupMasterMapper()
        {
            CreateMap<UpdateFlightGroupMasterRequest, FlightGroupMaster>();
        }
    }
}
