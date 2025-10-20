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

namespace tas.Application.Features.SysRoleReportTemplateFeature.GetAllSysRoleReportTemplate
{

    public sealed class GetAllSysRoleReportTemplateHandler : IRequestHandler<GetAllSysRoleReportTemplateRequest, List<GetAllSysRoleReportTemplateResponse>>
    {
        private readonly ISysRoleReportTemplateRepository _SysRoleReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetAllSysRoleReportTemplateHandler(ISysRoleReportTemplateRepository SysRoleReportTemplateRepository, IMapper mapper)
        {
            _SysRoleReportTemplateRepository = SysRoleReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllSysRoleReportTemplateResponse>> Handle(GetAllSysRoleReportTemplateRequest request, CancellationToken cancellationToken)
        {
            return await _SysRoleReportTemplateRepository.GetRoleReportTemplate(request, cancellationToken);
        }
    }
}
