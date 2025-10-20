using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.CreateState;
using tas.Application.Repositories;

namespace tas.Application.Features.SysTeamFeature.SetUserSysTeam
{
    public sealed class SetUserSysTeamHandler : IRequestHandler<SetUserSysTeamRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysTeamRepository _SysTeamRepository;
        private readonly IMapper _mapper;

        public SetUserSysTeamHandler(IUnitOfWork unitOfWork, ISysTeamRepository SysTeamRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysTeamRepository = SysTeamRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetUserSysTeamRequest requests, CancellationToken cancellationToken)
        {
            await _SysTeamRepository.SetUserSysTeam(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
