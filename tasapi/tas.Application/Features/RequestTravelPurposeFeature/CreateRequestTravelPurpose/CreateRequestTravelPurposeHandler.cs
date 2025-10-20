using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelPurposeFeature.CreateRequestTravelPurpose
{
    public sealed class CreateRequestTravelPurposeHandler : IRequestHandler<CreateRequestTravelPurposeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTravelPurposeRepository _RequestTravelPurposeRepository;
        private readonly IMapper _mapper;

        public CreateRequestTravelPurposeHandler(IUnitOfWork unitOfWork, IRequestTravelPurposeRepository RequestTravelPurposeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestTravelPurposeRepository = RequestTravelPurposeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRequestTravelPurposeRequest request, CancellationToken cancellationToken)
        {
            var RequestTravelPurpose = _mapper.Map<RequestTravelPurpose>(request);
            await _RequestTravelPurposeRepository.CheckDuplicateData(RequestTravelPurpose, c=> c.Code, c => c.Description);
            _RequestTravelPurposeRepository.Create(RequestTravelPurpose);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
