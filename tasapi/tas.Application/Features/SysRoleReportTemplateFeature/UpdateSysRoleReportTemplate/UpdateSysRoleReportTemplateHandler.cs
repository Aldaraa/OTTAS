using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate
{
    public sealed class UpdateSysRoleReportTemplateHandler : IRequestHandler<UpdateSysRoleReportTemplateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleReportTemplateRepository _SysRoleReportTemplateRepository;
        private readonly IMapper _mapper;

        public UpdateSysRoleReportTemplateHandler(IUnitOfWork unitOfWork, ISysRoleReportTemplateRepository SysRoleReportTemplateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleReportTemplateRepository = SysRoleReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateSysRoleReportTemplateRequest requests, CancellationToken cancellationToken)
        {
            await _SysRoleReportTemplateRepository.UpdateReportTemplateRole(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }



    }
}
