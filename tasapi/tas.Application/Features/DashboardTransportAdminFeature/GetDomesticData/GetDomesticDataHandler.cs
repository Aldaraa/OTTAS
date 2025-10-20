using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetDomesticData
{

    public sealed class GetDomesticDataHandler : IRequestHandler<GetDomesticDataRequest, GetDomesticDataResponse>
    {
        private readonly IDashboardTransportAdminRepository _dashboardTransportAdminRepository;
        private readonly IMapper _mapper;

        public GetDomesticDataHandler(IDashboardTransportAdminRepository dashboardTransportAdminRepository, IMapper mapper)
        {
            _dashboardTransportAdminRepository = dashboardTransportAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetDomesticDataResponse> Handle(GetDomesticDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardTransportAdminRepository.GetDomesticData(request, cancellationToken);
            return data;

        }
    }
}
