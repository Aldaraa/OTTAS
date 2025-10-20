using Application.Features.HotDeskFeature.EmployeeStatusInfo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeStatusInfo
{
    public sealed record EmployeeStatusInfoRequest :  IRequest<List<EmployeeStatusInfoResponse>>;





}
