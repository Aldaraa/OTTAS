using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu
{

    public sealed class GetAllSysRoleMenuHandler : IRequestHandler<GetAllSysRoleMenuRequest, List<GetAllSysRoleMenuResponse>>
    {
        private readonly ISysRoleMenuRepository _SysRoleMenuRepository;
        private readonly IMapper _mapper;

        public GetAllSysRoleMenuHandler(ISysRoleMenuRepository SysRoleMenuRepository, IMapper mapper)
        {
            _SysRoleMenuRepository = SysRoleMenuRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysRoleMenuResponse>> Handle(GetAllSysRoleMenuRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleMenuRepository.GetRoleMenu(request, cancellationToken);
        }
    }
}
