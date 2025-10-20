using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.BulkDownloadEmployee
{
    public sealed class BulkDownloadEmployeeHandler : IRequestHandler<BulkDownloadEmployeeRequest, BulkDownloadEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;

        public BulkDownloadEmployeeHandler(IEmployeeRepository EmployeeRepository)
        {
            _EmployeeRepository = EmployeeRepository;
        }

        public async Task<BulkDownloadEmployeeResponse>  Handle(BulkDownloadEmployeeRequest request, CancellationToken cancellationToken)
        {
          return await _EmployeeRepository.BulkRequestDownload(request, cancellationToken);
        }
    }
}
