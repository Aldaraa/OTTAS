using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardDataAdminFeature.GetEmployeeRegisterData
{

    public sealed class GetEmployeeRegisterDataHandler : IRequestHandler<GetEmployeeRegisterDataRequest, List<GetEmployeeRegisterDataResponse>>
    {
        private readonly IDashboardDataAdminRepository _dashboardDataAdminRepository;
        private readonly IMapper _mapper;

        public GetEmployeeRegisterDataHandler(IDashboardDataAdminRepository DashboardDataAdminRepository, IMapper mapper)
        {
            _dashboardDataAdminRepository = DashboardDataAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetEmployeeRegisterDataResponse>> Handle(GetEmployeeRegisterDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardDataAdminRepository.GetEmployeeRegisterData(request, cancellationToken);
            return data;

        }
    }
}
