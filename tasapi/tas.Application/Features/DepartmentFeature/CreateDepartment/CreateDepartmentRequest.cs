using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.CreateDepartment
{
    public sealed record CreateDepartmentRequest(string Name, int ParentDepartmentId, int? CostCodeId) : IRequest;
}
