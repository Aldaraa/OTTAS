using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleFeature.GetSysRole
{

    public sealed class GetSysRoleHandler : IRequestHandler<GetSysRoleRequest, GetSysRoleResponse>
    {
        private readonly ISysRoleRepository _SysRoleRepository;
        private readonly IMapper _mapper;

        public GetSysRoleHandler(ISysRoleRepository SysRoleRepository, IMapper mapper)
        {
            _SysRoleRepository = SysRoleRepository;
            _mapper = mapper;
        }

        public async Task<GetSysRoleResponse> Handle(GetSysRoleRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleRepository.GetData(request, cancellationToken);
        }
    }
}
