using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardTransportAdminFeature.GetTransportGroupEmployeeData
{

    public sealed class GetTransportGroupEmployeeDataHandler : IRequestHandler<GetTransportGroupEmployeeDataRequest,List<GetTransportGroupEmployeeDataResponse>>
    {
        private readonly IDashboardTransportAdminRepository _dashboardTransportAdminRepository;
        private readonly IMapper _mapper;

        public GetTransportGroupEmployeeDataHandler(IDashboardTransportAdminRepository dashboardTransportAdminRepository, IMapper mapper)
        {
            _dashboardTransportAdminRepository = dashboardTransportAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetTransportGroupEmployeeDataResponse>> Handle(GetTransportGroupEmployeeDataRequest request, CancellationToken cancellationToken)
        {
            var EmployeeData = await _dashboardTransportAdminRepository.GetTransportGroupEmployeeData(request, cancellationToken);
            return EmployeeData;

        }
    }
}
