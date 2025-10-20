using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleFeature.GetAllSysRole
{

    public sealed class GetAllSysRoleHandler : IRequestHandler<GetAllSysRoleRequest, List<GetAllSysRoleResponse>>
    {
        private readonly ISysRoleRepository _SysRoleRepository;
        private readonly IMapper _mapper;

        public GetAllSysRoleHandler(ISysRoleRepository SysRoleRepository, IMapper mapper)
        {
            _SysRoleRepository = SysRoleRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysRoleResponse>> Handle(GetAllSysRoleRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleRepository.GetAllData(cancellationToken);
        }
    }
}
