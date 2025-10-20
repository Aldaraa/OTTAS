using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployerFeature.CreateEmployer;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeStatusFeature.GetDateRangeStatus
{

    public sealed class GetDateRangeStatusHandler : IRequestHandler<GetDateRangeStatusRequest, List<GetDateRangeStatusResponse>>
    {
        private readonly IEmployeeStatusRepository _EmployeeStatusRepository;
        private readonly IMapper _mapper;

        public GetDateRangeStatusHandler(IEmployeeStatusRepository EmployeeStatusRepository, IMapper mapper)
        {
            _EmployeeStatusRepository = EmployeeStatusRepository;
            _mapper = mapper;
        }

        public async Task<List<GetDateRangeStatusResponse>> Handle(GetDateRangeStatusRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeStatusRepository.GetDateRangeStatus(request, cancellationToken);
            return data;


        }
    }
}
