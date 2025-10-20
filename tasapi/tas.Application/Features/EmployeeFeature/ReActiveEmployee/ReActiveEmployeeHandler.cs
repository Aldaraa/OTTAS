using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using tas.Application.Extensions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.ReActiveEmployee
{
    public sealed class ReActiveEmployeeHandler : IRequestHandler<ReActiveEmployeeRequest, Unit>
    {
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReActiveEmployeeHandler(IEmployeeRepository EmployeeRepository, IUnitOfWork unitOfWork)
        {
            _EmployeeRepository = EmployeeRepository;
            _unitOfWork = unitOfWork;   
        }

        public async Task<Unit> Handle(ReActiveEmployeeRequest request, CancellationToken cancellationToken)
        {
            await _EmployeeRepository.ReActiveEmployee(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
