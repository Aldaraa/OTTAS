using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu
{
    public sealed class UpdateSysRoleMenuHandler : IRequestHandler<UpdateSysRoleMenuRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleMenuRepository _SysRoleMenuRepository;
        private readonly IMapper _mapper;

        public UpdateSysRoleMenuHandler(IUnitOfWork unitOfWork, ISysRoleMenuRepository SysRoleMenuRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleMenuRepository = SysRoleMenuRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateSysRoleMenuRequest requests, CancellationToken cancellationToken)
        {
            await _SysRoleMenuRepository.UpdateMenuRole(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }



    }
}
