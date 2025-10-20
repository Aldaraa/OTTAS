using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelAgentFeature.DeleteRequestTravelAgent
{

    public sealed class DeleteRequestTravelAgentHandler : IRequestHandler<DeleteRequestTravelAgentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTravelAgentRepository _RequestTravelAgentRepository;
        private readonly IMapper _mapper;

        public DeleteRequestTravelAgentHandler(IUnitOfWork unitOfWork, IRequestTravelAgentRepository RequestTravelAgentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestTravelAgentRepository = RequestTravelAgentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestTravelAgentRequest request, CancellationToken cancellationToken)
        {
            var RequestTravelAgent = _mapper.Map<RequestTravelAgent>(request);
            _RequestTravelAgentRepository.Delete(RequestTravelAgent);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
