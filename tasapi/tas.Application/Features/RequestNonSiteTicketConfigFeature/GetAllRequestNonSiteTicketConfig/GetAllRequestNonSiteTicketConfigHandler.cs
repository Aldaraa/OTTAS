using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ColorFeature.GetAllColor;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestNonSiteTicketConfigFeature.GetAllRequestNonSiteTicketConfig
{

    public sealed class GetAllRequestNonSiteTicketConfigHandler : IRequestHandler<GetAllRequestNonSiteTicketConfigRequest, List<GetAllRequestNonSiteTicketConfigResponse>>
    {
        private readonly IRequestNonSiteTicketConfigRepository _RequestNonSiteTicketConfigRepository;
        private readonly IMapper _mapper;

        public GetAllRequestNonSiteTicketConfigHandler(IRequestNonSiteTicketConfigRepository RequestNonSiteTicketConfigRepository, IMapper mapper)
        {
            _RequestNonSiteTicketConfigRepository = RequestNonSiteTicketConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestNonSiteTicketConfigResponse>> Handle(GetAllRequestNonSiteTicketConfigRequest request, CancellationToken cancellationToken)
        {

            if (request.status.HasValue)
            {
                var Colors = await _RequestNonSiteTicketConfigRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllRequestNonSiteTicketConfigResponse>>(Colors);
            }
            else
            {
                var users = await _RequestNonSiteTicketConfigRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllRequestNonSiteTicketConfigResponse>>(users);
            }

        }
    }
}
