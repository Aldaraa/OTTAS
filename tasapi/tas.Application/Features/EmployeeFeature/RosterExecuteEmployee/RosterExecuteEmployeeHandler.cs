using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.RosterExecuteEmployee
{
    public sealed class RosterExecuteEmployeeHandler : IRequestHandler<RosterExecuteEmployeeRequest, List<RosterExecuteEmployeeResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public RosterExecuteEmployeeHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<List<RosterExecuteEmployeeResponse>>  Handle(RosterExecuteEmployeeRequest request, CancellationToken cancellationToken)
        {
           var data =  await  _EmployeeRepository.RosterExecute(request, cancellationToken);
            return data;
        }
    }
}
