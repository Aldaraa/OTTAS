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

namespace tas.Application.Features.EmployeeProfileStatusFeature.GetDateRangeProfileStatus
{

    public sealed class GetDateRangeProfileStatusHandler : IRequestHandler<GetDateRangeProfileStatusRequest, List<GetDateRangeProfileStatusResponse>>
    {
        private readonly IEmployeeStatusRepository _EmployeeProfileStatusRepository;
        private readonly IMapper _mapper;

        public GetDateRangeProfileStatusHandler(IEmployeeStatusRepository EmployeeProfileStatusRepository, IMapper mapper)
        {
            _EmployeeProfileStatusRepository = EmployeeProfileStatusRepository;
            _mapper = mapper;
        }

        public async Task<List<GetDateRangeProfileStatusResponse>> Handle(GetDateRangeProfileStatusRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeProfileStatusRepository.GetDateRangeProfileStatus(request, cancellationToken);
            return data;


        }
    }
}
