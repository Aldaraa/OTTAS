using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetPeopleTypeAndDepartment
{

    public sealed class GetPeopleTypeAndDepartmentHandler : IRequestHandler<GetPeopleTypeAndDepartmentRequest, GetPeopleTypeAndDepartmentResponse>
    {
        private readonly IDashboardSystemAdminRepository _dashboardSystemAdminRepository;
        private readonly IMapper _mapper;

        public GetPeopleTypeAndDepartmentHandler(IDashboardSystemAdminRepository dashboardSystemAdminRepository, IMapper mapper)
        {
            _dashboardSystemAdminRepository = dashboardSystemAdminRepository;
            _mapper = mapper;
        }

        public async Task<GetPeopleTypeAndDepartmentResponse> Handle(GetPeopleTypeAndDepartmentRequest request, CancellationToken cancellationToken)
        {
            var data = await _dashboardSystemAdminRepository.GetPeopleTypeAndDepartment(request, cancellationToken);
            return data;

        }
    }
}
