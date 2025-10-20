using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetOccupantsInfo
{

    public sealed class GetOccupantsInfoHandler : IRequestHandler<GetOccupantsInfoRequest, GetOccupantsInfoResponse>
    {
        private readonly IDashboardAccomAdminRepository _dashboardAccomAdminRepository;
        private readonly IMapper _mapper;

        public GetOccupantsInfoHandler(IDashboardAccomAdminRepository DashboardAccomAdminRepository, IMapper mapper)
        {
            _dashboardAccomAdminRepository = DashboardAccomAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetOccupantsInfoResponse> Handle(GetOccupantsInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardAccomAdminRepository.GetOccupantsInfo(request, cancellationToken);
            return data;

        }
    }
}
