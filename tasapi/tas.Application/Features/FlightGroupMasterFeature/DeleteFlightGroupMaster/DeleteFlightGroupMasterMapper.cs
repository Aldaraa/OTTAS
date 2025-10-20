using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.FlightGroupMasterFeature.UpdateFlightGroupMaster;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.DeleteFlightGroupMaster
{

    public sealed class DeleteFlightGroupMasterMapper : Profile
    {
        public DeleteFlightGroupMasterMapper()
        {
            CreateMap<DeleteFlightGroupMasterRequest, FlightGroupMaster>();
        }
    }
}
