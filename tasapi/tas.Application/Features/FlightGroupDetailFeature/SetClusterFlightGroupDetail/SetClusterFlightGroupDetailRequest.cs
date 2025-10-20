using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail
{
    public sealed record SetClusterFlightGroupDetailRequest(List<SetClusterFlightGroupDetail> request) : IRequest;

    public sealed record SetClusterFlightGroupDetail(int FlightGroupDetailId, int? ClusterId);

}
