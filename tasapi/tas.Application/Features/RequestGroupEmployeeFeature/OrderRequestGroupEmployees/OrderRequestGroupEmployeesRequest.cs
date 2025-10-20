using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.RequestGroupEmployeeFeature.OrderRequestGroupEmployees
{
    public sealed record OrderRequestGroupEmployeesRequest(int GroupId, List<int> Ids) : IRequest;
}
