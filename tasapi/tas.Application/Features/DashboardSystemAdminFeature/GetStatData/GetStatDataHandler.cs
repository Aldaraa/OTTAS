using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetStatData
{

    public sealed class GetStatDataHandler : IRequestHandler<GetStatDataRequest, List<GetStatDataResponse>>
    {
        private readonly IDashboardSystemAdminRepository _dashboardSystemAdminRepository;
        private readonly IMapper _mapper;

        public GetStatDataHandler(IDashboardSystemAdminRepository dashboardSystemAdminRepository, IMapper mapper)
        {
            _dashboardSystemAdminRepository = dashboardSystemAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetStatDataResponse>> Handle(GetStatDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardSystemAdminRepository.GetStatData(request, cancellationToken);
            return data;

        }
    }
}
