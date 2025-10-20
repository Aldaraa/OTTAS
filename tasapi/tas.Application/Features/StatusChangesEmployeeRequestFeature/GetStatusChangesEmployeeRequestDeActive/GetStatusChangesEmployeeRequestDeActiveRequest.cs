using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysMenuFeature.GetAllMenu;

    namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestDeActive
{
        public sealed record GetStatusChangesEmployeeRequestDeActiveRequest : IRequest<List<GetStatusChangesEmployeeRequestDeActiveResponse>>;

    }
