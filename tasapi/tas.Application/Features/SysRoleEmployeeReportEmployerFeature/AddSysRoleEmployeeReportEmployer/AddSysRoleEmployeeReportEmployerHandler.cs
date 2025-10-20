using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer
{

    public sealed class AddSysRoleEmployeeReportEmployerHandler : IRequestHandler<AddSysRoleEmployeeReportEmployerRequest,Unit>
    {
        private readonly ISysRoleEmployeeReportEmployerRepository _SysRoleEmployeeReportEmployerRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddSysRoleEmployeeReportEmployerHandler(ISysRoleEmployeeReportEmployerRepository SysRoleEmployeeReportEmployerRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _SysRoleEmployeeReportEmployerRepository = SysRoleEmployeeReportEmployerRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {
            await _SysRoleEmployeeReportEmployerRepository.AddSysRoleEmployeeReportEmployer(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
