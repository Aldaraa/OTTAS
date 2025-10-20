using AutoMapper;
using MediatR;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate
{

    public sealed class RequestDocumentGroupUpdateHandler : IRequestHandler<RequestDocumentGroupUpdateRequest, Unit>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RequestDocumentGroupUpdateHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RequestDocumentGroupUpdateRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupConfigRepository.UpdateApproval(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
