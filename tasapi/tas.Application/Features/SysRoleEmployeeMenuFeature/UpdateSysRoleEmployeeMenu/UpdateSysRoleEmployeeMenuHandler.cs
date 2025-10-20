using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeMenuFeature.UpdateSysRoleEmployeeMenu
{
    public sealed class UpdateSysRoleEmployeeMenuHandler : IRequestHandler<UpdateSysRoleEmployeeMenuRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleEmployeeMenuRepository _SysRoleEmployeeMenuRepository;
        private readonly IMapper _mapper;

        public UpdateSysRoleEmployeeMenuHandler(IUnitOfWork unitOfWork, ISysRoleEmployeeMenuRepository SysRoleEmployeeMenuRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleEmployeeMenuRepository = SysRoleEmployeeMenuRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateSysRoleEmployeeMenuRequest requests, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeMenuRepository.UpdateMenuRole(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }



    }
}
