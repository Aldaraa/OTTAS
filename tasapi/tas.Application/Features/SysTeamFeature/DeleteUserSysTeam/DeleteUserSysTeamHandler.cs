using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.SysTeamFeature.DeleteUserSysTeam
{
    public sealed class DeleteUserSysTeamHandler : IRequestHandler<DeleteUserSysTeamRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysTeamRepository _SysTeamRepository;
        private readonly IMapper _mapper;

        public DeleteUserSysTeamHandler(IUnitOfWork unitOfWork, ISysTeamRepository SysTeamRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysTeamRepository = SysTeamRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteUserSysTeamRequest requests, CancellationToken cancellationToken)
        {
            await _SysTeamRepository.DeleteUserSysTeam(requests, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
