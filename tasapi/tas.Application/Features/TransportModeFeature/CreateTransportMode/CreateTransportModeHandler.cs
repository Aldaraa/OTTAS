using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportModeFeature.CreateTransportMode
{
    public sealed class CreateTransportModeHandler : IRequestHandler<CreateTransportModeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportModeRepository _TransportModeRepository;
        private readonly IMapper _mapper;

        public CreateTransportModeHandler(IUnitOfWork unitOfWork, ITransportModeRepository TransportModeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportModeRepository = TransportModeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateTransportModeRequest request, CancellationToken cancellationToken)
        {
            var TransportMode = _mapper.Map<TransportMode>(request);
            await _TransportModeRepository.CheckDuplicateData(TransportMode, c => c.Code);
            _TransportModeRepository.Create(TransportMode);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
