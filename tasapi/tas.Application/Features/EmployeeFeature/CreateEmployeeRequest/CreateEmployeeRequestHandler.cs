using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.CreateEmployeeRequest
{
    public sealed class CreateEmployeeRequestHandler : IRequestHandler<CreateEmployeeRequestRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public CreateEmployeeRequestHandler(IUnitOfWork unitOfWork, IEmployeeRepository EmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
        }

        public async Task<int>  Handle(CreateEmployeeRequestRequest request, CancellationToken cancellationToken)
        {
            await  _EmployeeRepository.CreateEmployeeRequestValidateDB(request, cancellationToken);
            var Employee = _mapper.Map<Employee>(request);
            Employee.Active = 0;
            _EmployeeRepository.Create(Employee);
            await _unitOfWork.Save(cancellationToken);
            return Employee.Id;
     
        }
    }
}
