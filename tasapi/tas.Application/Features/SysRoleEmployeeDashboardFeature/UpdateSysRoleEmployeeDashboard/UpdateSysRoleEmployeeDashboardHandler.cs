using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeDashboardFeature.UpdateSysRoleEmployeeDashboard
{
    public sealed class UpdateSysRoleEmployeeDashboardHandler : IRequestHandler<UpdateSysRoleEmployeeDashboardRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleEmployeeDashboardRepository _SysRoleEmployeeDashboardRepository;
        private readonly IMapper _mapper;

        public UpdateSysRoleEmployeeDashboardHandler(IUnitOfWork unitOfWork, ISysRoleEmployeeDashboardRepository SysRoleEmployeeDashboardRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleEmployeeDashboardRepository = SysRoleEmployeeDashboardRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateSysRoleEmployeeDashboardRequest requests, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeDashboardRepository.UpdateDashboardRole(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }



    }
}
