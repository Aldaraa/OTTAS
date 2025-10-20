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

namespace tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket
{

    public sealed class ExtractOptioRequestNonSiteTicketHandler : IRequestHandler<ExtractOptioRequestNonSiteTicketRequest, ExtractOptioRequestNonSiteTicketResponse>
    {
        private readonly IRequestNonSiteTicketConfigRepository _RequestNonSiteTicketConfigRepository;
        private readonly IMapper _mapper;

        public ExtractOptioRequestNonSiteTicketHandler(IRequestNonSiteTicketConfigRepository RequestNonSiteTicketConfigRepository, IMapper mapper)
        {
            _RequestNonSiteTicketConfigRepository = RequestNonSiteTicketConfigRepository;
            _mapper = mapper;
        }

        public async Task<ExtractOptioRequestNonSiteTicketResponse> Handle(ExtractOptioRequestNonSiteTicketRequest request, CancellationToken cancellationToken)
        {
                var data = await _RequestNonSiteTicketConfigRepository.ExtractOption(request, cancellationToken);

            return data;
        }
    }
}
