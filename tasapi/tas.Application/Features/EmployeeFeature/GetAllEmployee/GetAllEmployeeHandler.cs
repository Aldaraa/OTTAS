using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.EmployeeFeature.GetAllEmployee
{

    public sealed class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeRequest, List<GetAllEmployeeResponse>>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public GetAllEmployeeHandler(IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllEmployeeResponse>> Handle(GetAllEmployeeRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var Employees = await _EmployeeRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllEmployeeResponse>>(Employees);
            }
            else {
                var users = await _EmployeeRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllEmployeeResponse>>(users);
            }

        }
    }
}
