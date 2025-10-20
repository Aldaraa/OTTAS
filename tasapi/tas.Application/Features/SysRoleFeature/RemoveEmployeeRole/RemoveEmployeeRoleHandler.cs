using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleFeature.RemoveEmployeeRole
{

    public sealed class RemoveEmployeeRoleHandler : IRequestHandler<RemoveEmployeeRoleRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleRepository _SysRoleRepository;
        private readonly IMapper _mapper;

        public RemoveEmployeeRoleHandler(IUnitOfWork unitOfWork, ISysRoleRepository SysRoleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleRepository = SysRoleRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(RemoveEmployeeRoleRequest request, CancellationToken cancellationToken)
        {
            await _SysRoleRepository.RemoveEmployee(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
