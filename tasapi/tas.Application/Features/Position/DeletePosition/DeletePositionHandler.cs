using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.DeletePosition
{

    public sealed class DeletePositionHandler : IRequestHandler<DeletePositionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public DeletePositionHandler(IUnitOfWork unitOfWork, IPositionRepository PositionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeletePositionRequest request, CancellationToken cancellationToken)
        {
            var Position = _mapper.Map<Position>(request);
            _PositionRepository.Delete(Position);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
