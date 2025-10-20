using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysMenuFeature.GetAllMenu;

    namespace tas.Application.Features.EmployerAdminFeature.GetEmployerAdmin
    {
        public sealed record GetEmployerAdminRequest(int EmployeeId) : IRequest<List<GetEmployerAdminResponse>>;

    }
