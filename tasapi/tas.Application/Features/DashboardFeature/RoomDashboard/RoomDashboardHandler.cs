using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardFeature.RoomDashboard
{
    public sealed class RoomDashboardHandler : IRequestHandler<RoomDashboardRequest, RoomDashboardResponse>
    {
        private readonly IDashboardRepository _DashboardRepository;
        private readonly IMapper _mapper;

        public RoomDashboardHandler(IDashboardRepository DashboardRepository, IMapper mapper)

        {
            _DashboardRepository = DashboardRepository;
            _mapper = mapper;
        }

        public async Task<RoomDashboardResponse> Handle(RoomDashboardRequest request, CancellationToken cancellationToken)
        {
            return await _DashboardRepository.RoomData(request, cancellationToken);


        }
    }
}
