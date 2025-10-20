using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupData
{

    public sealed class GetTransportGroupDataHandler : IRequestHandler<GetTransportGroupDataRequest,List<GetTransportGroupDataResponse>>
    {
        private readonly IDashboardTransportAdminRepository _dashboardTransportAdminRepository;
        private readonly IMapper _mapper;

        public GetTransportGroupDataHandler(IDashboardTransportAdminRepository dashboardTransportAdminRepository, IMapper mapper)
        {
            _dashboardTransportAdminRepository = dashboardTransportAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetTransportGroupDataResponse>> Handle(GetTransportGroupDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardTransportAdminRepository.GetTransportGroupData(request, cancellationToken);
            return data;

        }
    }
}
