using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLocalHotelFeature.DeleteRequestLocalHotel
{
    public sealed record DeleteRequestLocalHotelRequest(int Id) : IRequest;
}
