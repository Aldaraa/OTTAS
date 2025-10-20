using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.DeActiveEmployee
{
    public sealed class DeActiveEmployeeHandler : IRequestHandler<DeActiveEmployeeRequest, List<DeActiveEmployeeResponse>>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeActiveEmployeeHandler(IEmployeeRepository EmployeeRepository, IUnitOfWork unitOfWork)
        {
            _EmployeeRepository = EmployeeRepository;
            _unitOfWork = unitOfWork;   
        }

        public async Task<List<DeActiveEmployeeResponse>> Handle(DeActiveEmployeeRequest request, CancellationToken cancellationToken)
        {
           var returnData =  await _EmployeeRepository.DeActiveEmployee(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
