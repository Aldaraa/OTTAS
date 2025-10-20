using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardFeature.EmployeeDashboard
{
    public sealed class EmployeeDashboardHandler : IRequestHandler<EmployeeDashboardRequest, EmployeeDashboardResponse>
    {
        private readonly IDashboardRepository _DashboardRepository;
        private readonly IMapper _mapper;

        public EmployeeDashboardHandler(IDashboardRepository DashboardRepository, IMapper mapper)

        {
            _DashboardRepository = DashboardRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDashboardResponse> Handle(EmployeeDashboardRequest request, CancellationToken cancellationToken)
        {
            return await _DashboardRepository.EmployeeData(request, cancellationToken);


        }
    }
}
