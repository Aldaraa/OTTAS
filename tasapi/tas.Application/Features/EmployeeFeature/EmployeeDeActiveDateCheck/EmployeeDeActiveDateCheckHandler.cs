using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheck
{
    public sealed class EmployeeDeActiveDateCheckHandler : IRequestHandler<EmployeeDeActiveDateCheckRequest, EmployeeDeActiveDateCheckResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeDeActiveDateCheckHandler(IEmployeeRepository EmployeeRepository, IUnitOfWork unitOfWork)
        {
            _EmployeeRepository = EmployeeRepository;
            _unitOfWork = unitOfWork;   
        }

        public async Task<EmployeeDeActiveDateCheckResponse> Handle(EmployeeDeActiveDateCheckRequest request, CancellationToken cancellationToken)
        {
             var returnData =await _EmployeeRepository.EmployeeDeActiveDateCheck(request, cancellationToken);
            return returnData;
                 
        }
    }
}
