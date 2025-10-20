using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation
{

    public sealed class SearchEmployeeAccommodationHandler : IRequestHandler<SearchEmployeeAccommodationRequest, SearchEmployeeAccommodationResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public SearchEmployeeAccommodationHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<SearchEmployeeAccommodationResponse>Handle(SearchEmployeeAccommodationRequest request, CancellationToken cancellationToken)
        {
           return await _EmployeeRepository.SearchAccommodation(request, cancellationToken);
             

        }
    }
}
