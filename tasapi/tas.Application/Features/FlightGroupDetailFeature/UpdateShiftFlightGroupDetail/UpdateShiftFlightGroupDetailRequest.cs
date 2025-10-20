using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupDetailFeature.UpdateShiftFlightGroupDetail
{
    public sealed record UpdateShiftFlightGroupDetailRequest(int Id, int shiftid) : IRequest;

}
