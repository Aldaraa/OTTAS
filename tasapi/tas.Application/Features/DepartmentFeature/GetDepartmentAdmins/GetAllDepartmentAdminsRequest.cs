using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartmentAdmins
{
    public sealed record GetAllDepartmentAdminsRequest : IRequest<List<GetAllDepartmentAdminsResponse>>;
}
