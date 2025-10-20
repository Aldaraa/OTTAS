using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetCampUtilizationData
{

    public sealed class GetCampUtilizationDataHandler : IRequestHandler<GetCampUtilizationDataRequest, List<GetCampUtilizationDataResponse>>
    {
        private readonly IDashboardSystemAdminRepository _dashboardSystemAdminRepository;
        private readonly IMapper _mapper;

        public GetCampUtilizationDataHandler(IDashboardSystemAdminRepository dashboardSystemAdminRepository, IMapper mapper)
        {
            _dashboardSystemAdminRepository = dashboardSystemAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetCampUtilizationDataResponse>> Handle(GetCampUtilizationDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardSystemAdminRepository.GetCampUtilizationData(request, cancellationToken);
            return data;

        }
    }
}
