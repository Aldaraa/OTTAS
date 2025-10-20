using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportEmployerFeature.GetSysRoleEmployeeReportEmployer
{

    public sealed class GetSysRoleEmployeeReportEmployerHandler : IRequestHandler<GetSysRoleEmployeeReportEmployerRequest, List<GetSysRoleEmployeeReportEmployerResponse>>
    {
        private readonly ISysRoleEmployeeReportEmployerRepository _SysRoleEmployeeReportEmployerRepository;
        private readonly IMapper _mapper;

        public GetSysRoleEmployeeReportEmployerHandler(ISysRoleEmployeeReportEmployerRepository SysRoleEmployeeReportEmployerRepository, IMapper mapper)
        {
            _SysRoleEmployeeReportEmployerRepository = SysRoleEmployeeReportEmployerRepository;
            _mapper = mapper;
        }

        public async Task<List<GetSysRoleEmployeeReportEmployerResponse>> Handle(GetSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleEmployeeReportEmployerRepository.GetData(request, cancellationToken);
        }
    }
}
