using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardFeature.TransportDashboard
{
    public sealed class TransportDashboardHandler : IRequestHandler <TransportDashboardRequest,TransportDashboardResponse>
    {
        private readonly IDashboardRepository _DashboardRepository;
        private readonly IMapper _mapper;

        public TransportDashboardHandler(IDashboardRepository DashboardRepository, IMapper mapper)

        {
            _DashboardRepository = DashboardRepository;
            _mapper = mapper;
        }

        public async Task<TransportDashboardResponse> Handle(TransportDashboardRequest request, CancellationToken cancellationToken)
        {
            return await _DashboardRepository.TransportData(request, cancellationToken);


        }
    }
}
