using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardDataAdminFeature.GetProfileChangeDepartmentData
{

    public sealed class GetProfileChangeDepartmentDataHandler : IRequestHandler<GetProfileChangeDepartmentDataRequest, GetProfileChangeDepartmentDataResponse>
    {
        private readonly IDashboardDataAdminRepository _dashboardDataAdminRepository;
        private readonly IMapper _mapper;

        public GetProfileChangeDepartmentDataHandler(IDashboardDataAdminRepository DashboardDataAdminRepository, IMapper mapper)
        {
            _dashboardDataAdminRepository = DashboardDataAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetProfileChangeDepartmentDataResponse> Handle(GetProfileChangeDepartmentDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardDataAdminRepository.GetProfileChangeDepartmentData(request, cancellationToken);
            return data;

        }
    }
}
