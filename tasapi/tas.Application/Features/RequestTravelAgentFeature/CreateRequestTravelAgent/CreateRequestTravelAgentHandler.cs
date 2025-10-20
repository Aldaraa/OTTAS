using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelAgentFeature.CreateRequestTravelAgent
{
    public sealed class CreateRequestTravelAgentHandler : IRequestHandler<CreateRequestTravelAgentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTravelAgentRepository _RequestTravelAgentRepository;
        private readonly IMapper _mapper;

        public CreateRequestTravelAgentHandler(IUnitOfWork unitOfWork, IRequestTravelAgentRepository RequestTravelAgentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestTravelAgentRepository = RequestTravelAgentRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRequestTravelAgentRequest request, CancellationToken cancellationToken)
        {
            var RequestTravelAgentData = _mapper.Map<RequestTravelAgent>(request);
            await _RequestTravelAgentRepository.CheckDuplicateData(RequestTravelAgentData, c => c.Description);
            _RequestTravelAgentRepository.Create(RequestTravelAgentData);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
