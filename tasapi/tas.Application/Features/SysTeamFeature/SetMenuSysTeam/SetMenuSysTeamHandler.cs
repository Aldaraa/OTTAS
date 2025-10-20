using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Repositories;

namespace tas.Application.Features.SysTeamFeature.SetMenuSysTeam
{
    public sealed class SetMenuSysTeamHandler : IRequestHandler<SetMenuSysTeamBulkRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysTeamRepository _SysTeamRepository;
        private readonly IMapper _mapper;

        public SetMenuSysTeamHandler(IUnitOfWork unitOfWork, ISysTeamRepository SysTeamRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysTeamRepository = SysTeamRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetMenuSysTeamBulkRequest requests, CancellationToken cancellationToken)
        {
            await _SysTeamRepository.SetMenuSysTeam(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
