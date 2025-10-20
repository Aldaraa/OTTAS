using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.SearchShortEmployee
{

    public sealed class SearchShortEmployeeHandler : IRequestHandler<SearchShortEmployeeRequest, SearchShortEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public SearchShortEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<SearchShortEmployeeResponse>Handle(SearchShortEmployeeRequest request, CancellationToken cancellationToken)
        {
           return await _EmployeeRepository.SearchShortAdmin(request, cancellationToken);
             

        }
    }
}
