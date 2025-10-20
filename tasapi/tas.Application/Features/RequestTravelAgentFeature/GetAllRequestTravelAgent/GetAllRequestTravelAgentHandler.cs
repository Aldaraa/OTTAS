using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent
{

    public sealed class GetAllRequestTravelAgentHandler : IRequestHandler<GetAllRequestTravelAgentRequest, List<GetAllRequestTravelAgentResponse>>
    {
        private readonly IRequestTravelAgentRepository _RequestTravelAgentRepository;
        private readonly IMapper _mapper;

        public GetAllRequestTravelAgentHandler(IRequestTravelAgentRepository RequestTravelAgentRepository, IMapper mapper)
        {
            _RequestTravelAgentRepository = RequestTravelAgentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestTravelAgentResponse>> Handle(GetAllRequestTravelAgentRequest request, CancellationToken cancellationToken)
        {

           return await _RequestTravelAgentRepository.GetAllData(request, cancellationToken);

        }
    }
}
