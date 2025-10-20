using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheckMultiple
{
    public sealed class EmployeeDeActiveDateCheckMultipleHandler : IRequestHandler<EmployeeDeActiveDateCheckMultipleRequest, List<EmployeeDeActiveDateCheckMultipleResponse>>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeDeActiveDateCheckMultipleHandler(IEmployeeRepository EmployeeRepository, IUnitOfWork unitOfWork)
        {
            _EmployeeRepository = EmployeeRepository;
            _unitOfWork = unitOfWork;   
        }

        public async Task<List<EmployeeDeActiveDateCheckMultipleResponse>> Handle(EmployeeDeActiveDateCheckMultipleRequest request, CancellationToken cancellationToken)
        {
             var returnData =await _EmployeeRepository.EmployeeDeActiveDateCheckMultiple(request, cancellationToken);
            return returnData;
                 
        }
    }
}
