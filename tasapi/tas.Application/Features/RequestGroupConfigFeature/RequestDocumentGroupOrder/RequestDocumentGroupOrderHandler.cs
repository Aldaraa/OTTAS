using AutoMapper;
using MediatR;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupOrder
{

    public sealed class RequestDocumentGroupOrderHandler : IRequestHandler<RequestDocumentGroupOrderRequest, Unit>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RequestDocumentGroupOrderHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RequestDocumentGroupOrderRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupConfigRepository.OrderApproval(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
