using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BedFeature.CreateBed
{
    public sealed record CreateBedRequest(string Description, int RoomId) : IRequest;
}
