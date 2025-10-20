using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.AddSysRoleEmployeeReportDepartment
{

    public sealed class AddSysRoleEmployeeReportDepartmentHandler : IRequestHandler<AddSysRoleEmployeeReportDepartmentRequest,Unit>
    {
        private readonly ISysRoleEmployeeReportDepartmentRepository _SysRoleEmployeeReportDepartmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddSysRoleEmployeeReportDepartmentHandler(ISysRoleEmployeeReportDepartmentRepository SysRoleEmployeeReportDepartmentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _SysRoleEmployeeReportDepartmentRepository = SysRoleEmployeeReportDepartmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeReportDepartmentRepository.AddSysRoleEmployeeReportDepartment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
