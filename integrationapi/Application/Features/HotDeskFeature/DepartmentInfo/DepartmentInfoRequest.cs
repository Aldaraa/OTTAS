using Application.Features.HotDeskFeature.DepartmentInfo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HotDeskFeature.DepartmentInfo
{
    public sealed record DepartmentInfoRequest :  IRequest<List<DepartmentInfoResponse>>;





}
