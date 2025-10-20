using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetRosterData
{

    public sealed class GetRosterDataHandler : IRequestHandler<GetRosterDataRequest, GetRosterDataResponse>
    {
        private readonly IDashboardTransportAdminRepository _dashboardTransportAdminRepository;
        private readonly IMapper _mapper;

        public GetRosterDataHandler(IDashboardTransportAdminRepository dashboardTransportAdminRepository, IMapper mapper)
        {
            _dashboardTransportAdminRepository = dashboardTransportAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetRosterDataResponse> Handle(GetRosterDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardTransportAdminRepository.GetRosterData(request, cancellationToken);
            return data;

        }
    }
}
