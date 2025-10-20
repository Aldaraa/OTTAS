using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestTravelAgentFeature.UpdateRequestTravelAgent
{
    public class UpdateRequestTravelAgentHandler : IRequestHandler<UpdateRequestTravelAgentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTravelAgentRepository _RequestTravelAgentRepository;
        private readonly IMapper _mapper;

        public UpdateRequestTravelAgentHandler(IUnitOfWork unitOfWork, IRequestTravelAgentRepository RequestTravelAgentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestTravelAgentRepository = RequestTravelAgentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestTravelAgentRequest request, CancellationToken cancellationToken)
        {
            var RequestTravelAgent = _mapper.Map<RequestTravelAgent>(request);
            await _RequestTravelAgentRepository.CheckDuplicateData(RequestTravelAgent,  c => c.Description);
            _RequestTravelAgentRepository.Update(RequestTravelAgent);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
