using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate
{
    public sealed class UpdateSysRoleEmployeeReportTemplateHandler : IRequestHandler<UpdateSysRoleEmployeeReportTemplateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleEmployeeReportTemplateRepository _SysRoleEmployeeReportTemplateRepository;
        private readonly IMapper _mapper;

        public UpdateSysRoleEmployeeReportTemplateHandler(IUnitOfWork unitOfWork, ISysRoleEmployeeReportTemplateRepository SysRoleEmployeeReportTemplateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleEmployeeReportTemplateRepository = SysRoleEmployeeReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateSysRoleEmployeeReportTemplateRequest requests, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeReportTemplateRepository.UpdateReportTemplateRole(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }



    }
}
