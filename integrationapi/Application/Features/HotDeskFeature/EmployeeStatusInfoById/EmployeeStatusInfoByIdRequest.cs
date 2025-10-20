using Application.Features.HotDeskFeature.EmployeeStatusInfoById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeStatusInfoById
{
    public sealed record EmployeeStatusInfoByIdRequest(int employeeId) :  IRequest<List<EmployeeStatusInfoByIdResponse>>;





}
