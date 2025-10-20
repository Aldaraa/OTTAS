using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.SysRoleFeature.AddEmployee
{

    public sealed class AddEmployeeHandler : IRequestHandler<AddEmployeeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysRoleRepository _SysRoleRepository;
        private readonly IMapper _mapper;

        public AddEmployeeHandler(IUnitOfWork unitOfWork, ISysRoleRepository SysRoleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysRoleRepository = SysRoleRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(AddEmployeeRequest request, CancellationToken cancellationToken)
        {
            await _SysRoleRepository.AddEmployee(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
