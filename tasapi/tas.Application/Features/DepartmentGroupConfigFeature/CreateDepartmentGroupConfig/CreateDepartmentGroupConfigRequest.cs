using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentGroupConfigFeature.CreateDepartmentGroupConfig
{
    public sealed record CreateDepartmentGroupConfigRequest(int DepartmentId, List<int> EmployerIds, int GroupMasterId, int? GroupDetailId) : IRequest;
}
