using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardDataAdminFeature.GetOnsiteEmployeesData
{

    public sealed class GetOnsiteEmployeesDataHandler : IRequestHandler<GetOnsiteEmployeesDataRequest, GetOnsiteEmployeesDataResponse>
    {
        private readonly IDashboardDataAdminRepository _dashboardDataAdminRepository;
        private readonly IMapper _mapper;

        public GetOnsiteEmployeesDataHandler(IDashboardDataAdminRepository DashboardDataAdminRepository, IMapper mapper)
        {
            _dashboardDataAdminRepository = DashboardDataAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetOnsiteEmployeesDataResponse> Handle(GetOnsiteEmployeesDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardDataAdminRepository.GetOnsiteEmployeesData(request, cancellationToken);
            return data;

        }
    }
}
