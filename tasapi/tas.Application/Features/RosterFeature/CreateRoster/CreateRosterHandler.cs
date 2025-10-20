using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.CreateRoster
{
    public sealed class CreateRosterHandler : IRequestHandler<CreateRosterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterRepository _RosterRepository;
        private readonly IMapper _mapper;

        public CreateRosterHandler(IUnitOfWork unitOfWork, IRosterRepository RosterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterRepository = RosterRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRosterRequest request, CancellationToken cancellationToken)
        {
            var Roster = _mapper.Map<Roster>(request);
            await _RosterRepository.CheckDuplicateData(Roster, c => c.Name, c=> c.Description);
            _RosterRepository.Create(Roster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
