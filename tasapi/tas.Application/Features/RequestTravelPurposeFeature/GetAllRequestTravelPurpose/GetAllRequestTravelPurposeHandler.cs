using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestTravelPurposeFeature.GetAllRequestTravelPurpose
{

    public sealed class GetAllRequestTravelPurposeHandler : IRequestHandler<GetAllRequestTravelPurposeRequest, List<GetAllRequestTravelPurposeResponse>>
    {
        private readonly IRequestTravelPurposeRepository _RequestTravelPurposeRepository;
        private readonly IMapper _mapper;

        public GetAllRequestTravelPurposeHandler(IRequestTravelPurposeRepository RequestTravelPurposeRepository, IMapper mapper)
        {
            _RequestTravelPurposeRepository = RequestTravelPurposeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestTravelPurposeResponse>> Handle(GetAllRequestTravelPurposeRequest request, CancellationToken cancellationToken)
        {

           return await _RequestTravelPurposeRepository.GetAllData(request, cancellationToken);

        }
    }
}
