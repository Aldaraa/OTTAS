using AutoMapper;
using MediatR;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd
{

    public sealed class RequestDocumentGroupAddHandler : IRequestHandler<RequestDocumentGroupAddRequest, Unit>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RequestDocumentGroupAddHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RequestDocumentGroupAddRequest request, CancellationToken cancellationToken)
        {
            await _RequestGroupConfigRepository.AddApproval(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
