using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardDataAdminFeature.GetSeatBlockOnsiteData
{

    public sealed class GetSeatBlockOnsiteDataHandler : IRequestHandler<GetSeatBlockOnsiteDataRequest, List<GetSeatBlockOnsiteDataResponse>>
    {
        private readonly IDashboardDataAdminRepository _dashboardDataAdminRepository;
        private readonly IMapper _mapper;

        public GetSeatBlockOnsiteDataHandler(IDashboardDataAdminRepository DashboardDataAdminRepository, IMapper mapper)
        {
            _dashboardDataAdminRepository = DashboardDataAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetSeatBlockOnsiteDataResponse>> Handle(GetSeatBlockOnsiteDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardDataAdminRepository.GetSeatBlockOnsiteData(request, cancellationToken);
            return data;

        }
    }
}
