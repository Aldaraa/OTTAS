using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.DeleteTransportMode
{

    public sealed class DeleteTransportModeHandler : IRequestHandler<DeleteTransportModeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportModeRepository _TransportModeRepository;
        private readonly IMapper _mapper;

        public DeleteTransportModeHandler(IUnitOfWork unitOfWork, ITransportModeRepository TransportModeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportModeRepository = TransportModeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteTransportModeRequest request, CancellationToken cancellationToken)
        {
            var TransportMode = _mapper.Map<TransportMode>(request);
            _TransportModeRepository.Delete(TransportMode);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
