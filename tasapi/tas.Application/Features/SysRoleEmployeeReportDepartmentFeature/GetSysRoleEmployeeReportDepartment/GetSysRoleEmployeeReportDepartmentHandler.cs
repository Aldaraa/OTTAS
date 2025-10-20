using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.GetSysRoleEmployeeReportDepartment
{

    public sealed class GetSysRoleEmployeeReportDepartmentHandler : IRequestHandler<GetSysRoleEmployeeReportDepartmentRequest, List<GetSysRoleEmployeeReportDepartmentResponse>>
    {
        private readonly ISysRoleEmployeeReportDepartmentRepository _SysRoleEmployeeReportDepartmentRepository;
        private readonly IMapper _mapper;

        public GetSysRoleEmployeeReportDepartmentHandler(ISysRoleEmployeeReportDepartmentRepository SysRoleEmployeeReportDepartmentRepository, IMapper mapper)
        {
            _SysRoleEmployeeReportDepartmentRepository = SysRoleEmployeeReportDepartmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetSysRoleEmployeeReportDepartmentResponse>> Handle(GetSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleEmployeeReportDepartmentRepository.GetData(request, cancellationToken);
        }
    }
}
