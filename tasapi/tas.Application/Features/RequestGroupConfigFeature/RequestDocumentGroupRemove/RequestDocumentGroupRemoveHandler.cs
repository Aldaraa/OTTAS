using AutoMapper;
using MediatR;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove
{

    public sealed class RequestDocumentGroupRemoveHandler : IRequestHandler<RequestDocumentGroupRemoveRequest, Unit>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RequestDocumentGroupRemoveHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RequestDocumentGroupRemoveRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupConfigRepository.RemoveApproval(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
