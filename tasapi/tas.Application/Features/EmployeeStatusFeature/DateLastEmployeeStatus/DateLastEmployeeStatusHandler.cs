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

namespace tas.Application.Features.EmployeeStatusFeature.DateLastEmployeeStatus
{

    public sealed class DateLastEmployeeStatusHandler : IRequestHandler<DateLastEmployeeStatusRequest, DateLastEmployeeStatusResponse>
    {
        private readonly IEmployeeStatusRepository _EmployeeProfileStatusRepository;
        private readonly IMapper _mapper;

        public DateLastEmployeeStatusHandler(IEmployeeStatusRepository EmployeeProfileStatusRepository, IMapper mapper)
        {
            _EmployeeProfileStatusRepository = EmployeeProfileStatusRepository;
            _mapper = mapper;
        }

        public async Task<DateLastEmployeeStatusResponse> Handle(DateLastEmployeeStatusRequest request, CancellationToken cancellationToken)
        {

            var data = await _EmployeeProfileStatusRepository.DateLastEmployeeStatus(request, cancellationToken);
            return data;


        }
    }
}
