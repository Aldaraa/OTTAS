using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.SearchEmployee
{

    public sealed class SearchEmployeeHandler : IRequestHandler<SearchEmployeeRequest, SearchEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public SearchEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<SearchEmployeeResponse>Handle(SearchEmployeeRequest request, CancellationToken cancellationToken)
        {
           return await _EmployeeRepository.SearchAdmin(request, cancellationToken);
             

        }
    }
}
