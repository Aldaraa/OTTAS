using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.CreateActiveTransport
{
    public sealed class CreateActiveTransportHandler : IRequestHandler<CreateActiveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public CreateActiveTransportHandler(IUnitOfWork unitOfWork, IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var ActiveTransport = _mapper.Map<ActiveTransport>(request);
            _ActiveTransportRepository.Create(ActiveTransport);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
