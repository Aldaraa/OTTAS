using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetCampUsageInfo
{

    public sealed class GetCampUsageInfoHandler : IRequestHandler<GetCampUsageInfoRequest, GetCampUsageInfoResponse>
    {
        private readonly IDashboardAccomAdminRepository _dashboardAccomAdminRepository;
        private readonly IMapper _mapper;

        public GetCampUsageInfoHandler(IDashboardAccomAdminRepository DashboardAccomAdminRepository, IMapper mapper)
        {
            _dashboardAccomAdminRepository = DashboardAccomAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetCampUsageInfoResponse> Handle(GetCampUsageInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardAccomAdminRepository.GetCampUsageInfo(request, cancellationToken);
            return data;

        }
    }
}
