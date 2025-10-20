using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetCampPOBData
{
    public sealed record GetCampPOBDataRequest(bool weekly = true) :  IRequest<List<GetCampPOBDataResponse>>;

}
