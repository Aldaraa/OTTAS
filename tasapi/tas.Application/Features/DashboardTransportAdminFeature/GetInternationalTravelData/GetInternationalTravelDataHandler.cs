using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetInternationalTravelData
{

    public sealed class GetInternationalTravelDataHandler : IRequestHandler<GetInternationalTravelDataRequest, GetInternationalTravelDataResponse>
    {
        private readonly IDashboardTransportAdminRepository _dashboardTransportAdminRepository;
        private readonly IMapper _mapper;

        public GetInternationalTravelDataHandler(IDashboardTransportAdminRepository dashboardTransportAdminRepository, IMapper mapper)
        {
            _dashboardTransportAdminRepository = dashboardTransportAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetInternationalTravelDataResponse> Handle(GetInternationalTravelDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardTransportAdminRepository.GetInternationalTravelData(request, cancellationToken);
            return data;

        }
    }
}
