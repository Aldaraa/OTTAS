using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Domain.Common;

namespace tas.Application.Features.PositionFeature.GetAllPosition
{
    public sealed record GetAllPositionRequest(int? Active, string? Code, string? Description) : BasePagenationRequest, IRequest<GetAllPositionResponse>;

}
