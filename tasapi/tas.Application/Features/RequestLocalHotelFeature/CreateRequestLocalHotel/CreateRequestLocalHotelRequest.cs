using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLocalHotelFeature.CreateRequestLocalHotel
{
    public sealed record CreateRequestLocalHotelRequest(string Description, int Active, decimal? DayCost,
        decimal? EarlyCheckInCost,
        decimal? LateCheckOutCost) : IRequest;
}
