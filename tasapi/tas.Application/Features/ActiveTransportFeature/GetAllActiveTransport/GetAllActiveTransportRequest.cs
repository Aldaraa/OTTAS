using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Domain.Common;

namespace tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport
{
    public sealed record GetAllActiveTransportRequest(
        GetAllActiveTransportRequestModel model
        ) : BasePagenationRequest, IRequest<GetAllActiveTransportResponse>;


   public sealed record GetAllActiveTransportRequestModel (
               int? FromLocationId,
        int? ToLocationId,
        DateTime? ScheduleDate,
        string? DayNum,
        string? TransportCode,
        int? active,
        int? TransportModeId
       );

}
