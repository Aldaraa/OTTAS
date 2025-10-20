using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo
{
    public sealed record GetCampInfoRequest :  IRequest<GetCampInfoResponse>;



}
