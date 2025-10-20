using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupMasterFeature.UpdateFlightGroupMaster
{
    public sealed record UpdateFlightGroupMasterRequest(int Id, string Code, string Description, int Active) : IRequest;
}
