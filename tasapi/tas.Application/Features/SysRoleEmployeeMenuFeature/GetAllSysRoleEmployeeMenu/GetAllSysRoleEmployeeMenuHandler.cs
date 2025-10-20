using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu
{

    public sealed class GetAllSysRoleEmployeeMenuHandler : IRequestHandler<GetAllSysRoleEmployeeMenuRequest, List<GetAllSysRoleEmployeeMenuResponse>>
    {
        private readonly ISysRoleEmployeeMenuRepository _SysRoleEmployeeMenuRepository;
        private readonly IMapper _mapper;

        public GetAllSysRoleEmployeeMenuHandler(ISysRoleEmployeeMenuRepository SysRoleEmployeeMenuRepository, IMapper mapper)
        {
            _SysRoleEmployeeMenuRepository = SysRoleEmployeeMenuRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysRoleEmployeeMenuResponse>> Handle(GetAllSysRoleEmployeeMenuRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleEmployeeMenuRepository.GetRoleMenu(request, cancellationToken);
        }
    }
}
