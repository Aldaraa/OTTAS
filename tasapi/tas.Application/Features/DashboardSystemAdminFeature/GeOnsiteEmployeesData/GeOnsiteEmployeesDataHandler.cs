using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardSystemAdminFeature.GeOnsiteEmployeesData
{

    public sealed class GeOnsiteEmployeesDataHandler : IRequestHandler<GeOnsiteEmployeesDataRequest, GeOnsiteEmployeesDataResponse>
    {
        private readonly IDashboardSystemAdminRepository _dashboardSystemAdminRepository;
        private readonly IMapper _mapper;

        public GeOnsiteEmployeesDataHandler(IDashboardSystemAdminRepository dashboardSystemAdminRepository, IMapper mapper)
        {
            _dashboardSystemAdminRepository = dashboardSystemAdminRepository;
            _mapper = mapper;
        }

        public async Task<GeOnsiteEmployeesDataResponse> Handle(GeOnsiteEmployeesDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardSystemAdminRepository.GeOnsiteEmployeesData(request, cancellationToken);
            return data;

        }
    }
}
