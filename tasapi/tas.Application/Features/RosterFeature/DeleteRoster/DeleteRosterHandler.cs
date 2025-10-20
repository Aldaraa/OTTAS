using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.DeleteRoster
{

    public sealed class DeleteRosterHandler : IRequestHandler<DeleteRosterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterRepository _RosterRepository;
        private readonly IMapper _mapper;

        public DeleteRosterHandler(IUnitOfWork unitOfWork, IRosterRepository RosterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterRepository = RosterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRosterRequest request, CancellationToken cancellationToken)
        {
            var Roster = _mapper.Map<Roster>(request);
            _RosterRepository.Delete(Roster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
