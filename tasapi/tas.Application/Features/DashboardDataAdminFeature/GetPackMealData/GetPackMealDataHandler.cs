using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardDataAdminFeature.GetPackMealData
{

    public sealed class GetPackMealDataHandler : IRequestHandler<GetPackMealDataRequest, List<GetPackMealDataResponse>>
    {
        private readonly IDashboardDataAdminRepository _dashboardDataAdminRepository;
        private readonly IMapper _mapper;

        public GetPackMealDataHandler(IDashboardDataAdminRepository DashboardDataAdminRepository, IMapper mapper)
        {
            _dashboardDataAdminRepository = DashboardDataAdminRepository;
            _mapper = mapper;
        }

        public async Task<List<GetPackMealDataResponse>> Handle(GetPackMealDataRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardDataAdminRepository.GetPackMealData(request, cancellationToken);
            return data;

        }
    }
}
