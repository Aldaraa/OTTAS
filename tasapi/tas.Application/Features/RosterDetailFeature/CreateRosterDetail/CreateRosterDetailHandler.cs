using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterDetailFeature.CreateRoster
{
    public sealed class CreateRosterDetailHandler : IRequestHandler<CreateRosterDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterDetailRepository _RosterDetailRepository;
        private readonly IMapper _mapper;

        public CreateRosterDetailHandler(IUnitOfWork unitOfWork, IRosterDetailRepository RosterDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterDetailRepository = RosterDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRosterDetailRequest request, CancellationToken cancellationToken)
        {
            var RosterDetail = _mapper.Map<RosterDetail>(request);
            _RosterDetailRepository.Create(RosterDetail);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
