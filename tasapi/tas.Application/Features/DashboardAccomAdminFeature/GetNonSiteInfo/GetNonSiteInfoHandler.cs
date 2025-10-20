using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetNonSiteInfo
{

    public sealed class GetNonSiteInfoHandler : IRequestHandler<GetNonSiteInfoRequest, GetNonSiteInfoResponse>
    {
        private readonly IDashboardAccomAdminRepository _dashboardAccomAdminRepository;
        private readonly IMapper _mapper;

        public GetNonSiteInfoHandler(IDashboardAccomAdminRepository DashboardAccomAdminRepository, IMapper mapper)
        {
            _dashboardAccomAdminRepository = DashboardAccomAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetNonSiteInfoResponse> Handle(GetNonSiteInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardAccomAdminRepository.GetNonSiteInfo(request, cancellationToken);
            return data;

        }
    }
}
