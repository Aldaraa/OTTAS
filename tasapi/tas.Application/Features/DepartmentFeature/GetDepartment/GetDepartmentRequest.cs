using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentFeature.GetDepartment
{

    public sealed record GetDepartmentRequest(int Id) : IRequest<GetDepartmentResponse>;

   
}
