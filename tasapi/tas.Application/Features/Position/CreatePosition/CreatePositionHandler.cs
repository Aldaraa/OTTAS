using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.CreatePosition
{
    public sealed class CreatePositionHandler : IRequestHandler<CreatePositionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public CreatePositionHandler(IUnitOfWork unitOfWork, IPositionRepository PositionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreatePositionRequest request, CancellationToken cancellationToken)
        {
            var Position = _mapper.Map<Position>(request);
            await _PositionRepository.CheckDuplicateData(Position, c => c.Code, c => c.Description);
            _PositionRepository.Create(Position);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
