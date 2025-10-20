using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLocalHotelFeature.UpdateRequestLocalHotel
{
    public sealed record UpdateRequestLocalHotelRequest(int Id, string Description, decimal? DayCost,
        decimal? EarlyCheckInCost,
        decimal? LateCheckOutCost) : IRequest;
}
