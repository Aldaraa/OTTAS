using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.UpdateDepartment
{
    public sealed record UpdateDepartmentRequest(int Id, string Name, int? ParentDepartmentId, int Active, int? CostCodeId) : IRequest;
}
