using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentGroupConfigFeature.DeleteDepartmentGroupConfig
{
    public sealed record DeleteDepartmentGroupConfigRequest(List<int> Ids) : IRequest;
}
