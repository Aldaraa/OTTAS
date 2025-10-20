using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.RequestAirportFeature.GetAllRequestAirport
{
    public sealed record GetAllRequestAirportRequest(int? status) : IRequest<List<GetAllRequestAirportResponse>>;
}

