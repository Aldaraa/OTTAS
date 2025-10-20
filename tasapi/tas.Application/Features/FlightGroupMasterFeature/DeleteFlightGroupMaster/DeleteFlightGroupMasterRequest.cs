using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.DeleteFlightGroupMaster
{
    public sealed record DeleteFlightGroupMasterRequest(int Id) : IRequest;
}
