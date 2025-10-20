using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate
{

    public sealed class GetAllSysRoleEmployeeReportTemplateHandler : IRequestHandler<GetAllSysRoleEmployeeReportTemplateRequest, List<GetAllSysRoleEmployeeReportTemplateResponse>>
    {
        private readonly ISysRoleEmployeeReportTemplateRepository _SysRoleEmployeeReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetAllSysRoleEmployeeReportTemplateHandler(ISysRoleEmployeeReportTemplateRepository SysRoleEmployeeReportTemplateRepository, IMapper mapper)
        {
            _SysRoleEmployeeReportTemplateRepository = SysRoleEmployeeReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysRoleEmployeeReportTemplateResponse>> Handle(GetAllSysRoleEmployeeReportTemplateRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleEmployeeReportTemplateRepository.GetEmployeeReportTemplate(request, cancellationToken);
        }
    }
}
