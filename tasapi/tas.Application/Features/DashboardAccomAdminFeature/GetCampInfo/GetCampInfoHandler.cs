using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo
{

    public sealed class GetCampInfoHandler : IRequestHandler<GetCampInfoRequest, GetCampInfoResponse>
    {
        private readonly IDashboardAccomAdminRepository _dashboardAccomAdminRepository;
        private readonly IMapper _mapper;

        public GetCampInfoHandler(IDashboardAccomAdminRepository DashboardAccomAdminRepository, IMapper mapper)
        {
            _dashboardAccomAdminRepository = DashboardAccomAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetCampInfoResponse> Handle(GetCampInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardAccomAdminRepository.GetCampInfo(request, cancellationToken);
            return data;

        }
    }
}
