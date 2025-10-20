using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelPurposeFeature.DeleteRequestTravelPurpose
{

    public sealed class DeleteRequestTravelPurposeHandler : IRequestHandler<DeleteRequestTravelPurposeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTravelPurposeRepository _RequestTravelPurposeRepository;
        private readonly IMapper _mapper;

        public DeleteRequestTravelPurposeHandler(IUnitOfWork unitOfWork, IRequestTravelPurposeRepository RequestTravelPurposeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestTravelPurposeRepository = RequestTravelPurposeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestTravelPurposeRequest request, CancellationToken cancellationToken)
        {
            var RequestTravelPurposeData = _mapper.Map<RequestTravelPurpose>(request);
            _RequestTravelPurposeRepository.Delete(RequestTravelPurposeData);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
