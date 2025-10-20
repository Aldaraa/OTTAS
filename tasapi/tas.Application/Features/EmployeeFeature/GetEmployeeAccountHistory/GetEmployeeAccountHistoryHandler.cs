using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.GetEmployeeAccountHistory
{

    public sealed class GetEmployeeAccountHistoryHandler : IRequestHandler<GetEmployeeAccountHistoryRequest, List<GetEmployeeAccountHistoryResponse>>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeAccountHistoryHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetEmployeeAccountHistoryResponse>> Handle(GetEmployeeAccountHistoryRequest request, CancellationToken cancellationToken)
        {
            var data =  await _EmployeeRepository.GetAccountHistory(request, cancellationToken);
            return data;

        }
    }
}
