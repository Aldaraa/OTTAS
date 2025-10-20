using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NotificationFeature.NotificationGetAll;
using tas.Domain.Common;

namespace tas.Application.Features.NotificationFeature.NotificationGetAll
{
    public sealed record NotificationGetAllRequest : BasePagenationRequest, IRequest<NotificationGetAllResponse>;
}
