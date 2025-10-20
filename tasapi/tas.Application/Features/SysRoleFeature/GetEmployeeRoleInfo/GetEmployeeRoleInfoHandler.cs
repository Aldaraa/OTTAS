using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleFeature.GetEmployeeRoleInfo
{

    public sealed class GetEmployeeRoleInfoHandler : IRequestHandler<GetEmployeeRoleInfoRequest, GetEmployeeRoleInfoResponse>
    {
        private readonly ISysRoleRepository _SysRoleRepository;
        private readonly IMapper _mapper;

        public GetEmployeeRoleInfoHandler(ISysRoleRepository SysRoleRepository, IMapper mapper)
        {
            _SysRoleRepository = SysRoleRepository;
            _mapper = mapper;
        }

        public async Task<GetEmployeeRoleInfoResponse> Handle(GetEmployeeRoleInfoRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleRepository.GetRoleInfoData(request, cancellationToken);
        }
    }
}
