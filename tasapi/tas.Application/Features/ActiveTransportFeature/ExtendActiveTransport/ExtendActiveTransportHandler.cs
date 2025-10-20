using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ActiveTransportFeature.ExtendActiveTransport
{
    public sealed class ExtendActiveTransportHandler : IRequestHandler<ExtendActiveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public ExtendActiveTransportHandler(IUnitOfWork unitOfWork, IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(ExtendActiveTransportRequest request, CancellationToken cancellationToken)
        {
            await _ActiveTransportRepository.Extend(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
