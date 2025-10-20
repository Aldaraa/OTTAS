using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetNoRoomInfo
{

    public sealed class GetNoRoomInfoHandler : IRequestHandler<GetNoRoomInfoRequest, GetNoRoomInfoResponse>
    {
        private readonly IDashboardAccomAdminRepository _dashboardAccomAdminRepository;
        private readonly IMapper _mapper;

        public GetNoRoomInfoHandler(IDashboardAccomAdminRepository DashboardAccomAdminRepository, IMapper mapper)
        {
            _dashboardAccomAdminRepository = DashboardAccomAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetNoRoomInfoResponse> Handle(GetNoRoomInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardAccomAdminRepository.GetNoRoomInfo(request, cancellationToken);
            return data;

        }
    }
}
