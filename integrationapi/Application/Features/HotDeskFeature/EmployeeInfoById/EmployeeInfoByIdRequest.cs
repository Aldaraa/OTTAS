using Application.Features.HotDeskFeature.EmployeeInfoById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeInfoById
{
    public sealed record EmployeeInfoByIdRequest(int employeeId) :  IRequest<EmployeeInfoByIdResponse>;





}
