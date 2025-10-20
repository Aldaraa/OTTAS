using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.CreateFlightGroupMaster
{
    public sealed record CreateFlightGroupMasterRequest(string Code, string Description, int DayPattern) : IRequest;
}
