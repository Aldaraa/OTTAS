using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetEmployeeTransportData
{

    public sealed class GetEmployeeTransportDataHandler : IRequestHandler<GetEmployeeTransportDataRequest, GetEmployeeTransportDataResponse>
    {
        private readonly IDashboardSystemAdminRepository _dashboardSystemAdminRepository;
        private readonly IMapper _mapper;

        public GetEmployeeTransportDataHandler(IDashboardSystemAdminRepository dashboardSystemAdminRepository, IMapper mapper)
        {
            _dashboardSystemAdminRepository = dashboardSystemAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetEmployeeTransportDataResponse> Handle(GetEmployeeTransportDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardSystemAdminRepository.GetEmployeeTransportData(request, cancellationToken);
            return data;

        }
    }
}
