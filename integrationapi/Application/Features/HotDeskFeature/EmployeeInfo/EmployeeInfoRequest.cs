using Application.Features.HotDeskFeature.EmployeeInfo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.EmployeeInfo
{
    public sealed record EmployeeInfoRequest :  IRequest<List<EmployeeInfoResponse>>;





}
