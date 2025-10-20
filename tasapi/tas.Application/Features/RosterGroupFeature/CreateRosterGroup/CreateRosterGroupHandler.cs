using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterGroupFeature.CreateRosterGroup
{
    public sealed class CreateRosterGroupHandler : IRequestHandler<CreateRosterGroupRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterGroupRepository _RosterGroupRepository;
        private readonly IMapper _mapper;

        public CreateRosterGroupHandler(IUnitOfWork unitOfWork, IRosterGroupRepository RosterGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterGroupRepository = RosterGroupRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRosterGroupRequest request, CancellationToken cancellationToken)
        {
            var RosterGroup = _mapper.Map<RosterGroup>(request);
            _RosterGroupRepository.Create(RosterGroup);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
