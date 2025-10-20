using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.DepartmentFeature.GetAllDepartment
{
    public sealed record GetAllDepartmentRequest(string? departmentName, string keyword) : IRequest<List<GetAllDepartmentResponse>>;
}
