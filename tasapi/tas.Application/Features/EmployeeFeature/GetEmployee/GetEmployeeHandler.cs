using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.GetEmployee
{

    public sealed class GetEmployeeHandler : IRequestHandler<GetEmployeeRequest, GetEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<GetEmployeeResponse> Handle(GetEmployeeRequest request, CancellationToken cancellationToken)
        {
            var Employees = await _EmployeeRepository.GetProfileAdmin(request.Id, cancellationToken);
            return _mapper.Map<GetEmployeeResponse>(Employees);

        }
    }
}
