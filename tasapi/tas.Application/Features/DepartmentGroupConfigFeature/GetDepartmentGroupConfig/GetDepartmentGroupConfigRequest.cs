using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentGroupConfigFeature.GetDepartmentGroupConfig
{
    public sealed record GetDepartmentGroupConfigRequest(int DepartentId) : IRequest<List<GetDepartmentGroupConfigResponse>>;
}
