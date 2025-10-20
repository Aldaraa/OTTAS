using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster
{
    public sealed record GetFlightGroupMasterRequest(int Id) : IRequest<GetFlightGroupMasterResponse>;
}
