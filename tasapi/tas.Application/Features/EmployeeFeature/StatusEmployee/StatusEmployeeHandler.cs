using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.StatusEmployee
{

    public sealed class StatusEmployeeHandler : IRequestHandler<StatusEmployeeRequest, StatusEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public StatusEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<StatusEmployeeResponse>Handle(StatusEmployeeRequest request, CancellationToken cancellationToken)
        {
           return await _EmployeeRepository.GetStatusDates(request, cancellationToken);
             

        }
    }
}
