using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NotificationFeature.NotificationGetDetail;

namespace tas.Application.Features.NotificationFeature.NotificationGetDetail
{
    public sealed record NotificationGetDetailRequest(string NotifIndex) : IRequest<NotificationGetDetailResponse>;
}
