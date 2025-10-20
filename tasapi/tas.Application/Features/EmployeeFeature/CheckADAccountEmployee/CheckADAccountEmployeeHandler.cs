using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.CheckADAccountEmployee
{
    public sealed class CheckADAccountEmployeeHandler : IRequestHandler<CheckADAccountEmployeeRequest, CheckADAccountEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public CheckADAccountEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<CheckADAccountEmployeeResponse> Handle(CheckADAccountEmployeeRequest request, CancellationToken cancellationToken)
        {
          return await _EmployeeRepository.CheckADAccount(request, cancellationToken);

        }
    }
}
