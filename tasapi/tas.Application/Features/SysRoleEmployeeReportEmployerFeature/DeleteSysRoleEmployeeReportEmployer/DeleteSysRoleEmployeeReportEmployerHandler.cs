using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportEmployerFeature.DeleteSysRoleEmployeeReportEmployer
{

    public sealed class DeleteSysRoleEmployeeReportEmployerHandler : IRequestHandler<DeleteSysRoleEmployeeReportEmployerRequest,Unit>
    {
        private readonly ISysRoleEmployeeReportEmployerRepository _SysRoleEmployeeReportEmployerRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSysRoleEmployeeReportEmployerHandler(ISysRoleEmployeeReportEmployerRepository SysRoleEmployeeReportEmployerRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _SysRoleEmployeeReportEmployerRepository = SysRoleEmployeeReportEmployerRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeReportEmployerRepository.DeleteSysRoleEmployeeReportEmployer(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
