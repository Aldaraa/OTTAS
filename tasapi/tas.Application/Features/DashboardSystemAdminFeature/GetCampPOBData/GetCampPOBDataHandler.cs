using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetCampPOBData
{

    public sealed class GetCampPOBDataHandler : IRequestHandler<GetCampPOBDataRequest, List<GetCampPOBDataResponse>>
    {
        private readonly IDashboardSystemAdminRepository _dashboardSystemAdminRepository;
        private readonly IMapper _mapper;

        public GetCampPOBDataHandler(IDashboardSystemAdminRepository dashboardSystemAdminRepository, IMapper mapper)
        {
            _dashboardSystemAdminRepository = dashboardSystemAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetCampPOBDataResponse>> Handle(GetCampPOBDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardSystemAdminRepository.GetCampPOBData(request, cancellationToken);
            return data;

        }
    }
}
