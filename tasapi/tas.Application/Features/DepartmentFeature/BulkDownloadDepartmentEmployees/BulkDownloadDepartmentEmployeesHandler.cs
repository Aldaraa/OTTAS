using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.BulkDownloadDepartmentEmployees
{
    public sealed class BulkDownloadDepartmentEmployeesHandler : IRequestHandler<BulkDownloadDepartmentEmployeesRequest, BulkDownloadDepartmentEmployeesResponse>
    {
        private readonly IDepartmentRepository _DepartmentRepository;

        public BulkDownloadDepartmentEmployeesHandler(IDepartmentRepository DepartmentRepository)
        {
            _DepartmentRepository = DepartmentRepository;
        }

        public async Task<BulkDownloadDepartmentEmployeesResponse>  Handle(BulkDownloadDepartmentEmployeesRequest request, CancellationToken cancellationToken)
        {
          return await _DepartmentRepository.BulkRequestDownloadEmployees(request, cancellationToken);
        }
    }
}
