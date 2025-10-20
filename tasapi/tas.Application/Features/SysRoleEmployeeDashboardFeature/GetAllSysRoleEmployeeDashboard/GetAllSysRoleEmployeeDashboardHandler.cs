using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeDashboardFeature.GetAllSysRoleEmployeeDashboard
{

    public sealed class GetAllSysRoleEmployeeDashboardHandler : IRequestHandler<GetAllSysRoleEmployeeDashboardRequest, List<GetAllSysRoleEmployeeDashboardResponse>>
    {
        private readonly ISysRoleEmployeeDashboardRepository _SysRoleEmployeeDashboardRepository;
        private readonly IMapper _mapper;

        public GetAllSysRoleEmployeeDashboardHandler(ISysRoleEmployeeDashboardRepository SysRoleEmployeeDashboardRepository, IMapper mapper)
        {
            _SysRoleEmployeeDashboardRepository = SysRoleEmployeeDashboardRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysRoleEmployeeDashboardResponse>> Handle(GetAllSysRoleEmployeeDashboardRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleEmployeeDashboardRepository.GetRoleDashboard(request, cancellationToken);
        }
    }
}
