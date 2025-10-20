using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.DeleteSysRoleEmployeeReportDepartment
{

    public sealed class DeleteSysRoleEmployeeReportDepartmentHandler : IRequestHandler<DeleteSysRoleEmployeeReportDepartmentRequest,Unit>
    {
        private readonly ISysRoleEmployeeReportDepartmentRepository _SysRoleEmployeeReportDepartmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSysRoleEmployeeReportDepartmentHandler(ISysRoleEmployeeReportDepartmentRepository SysRoleEmployeeReportDepartmentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _SysRoleEmployeeReportDepartmentRepository = SysRoleEmployeeReportDepartmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeReportDepartmentRepository.DeleteSysRoleEmployeeReportDepartment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
