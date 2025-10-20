using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.BulkDownloadGroupEmployee
{
    public sealed class BulkDownloadGroupEmployeeHandler : IRequestHandler<BulkDownloadGroupEmployeeRequest, BulkDownloadGroupEmployeeResponse>
    {
        private readonly IEmployeeRepository _EmployeeRepository;

        public BulkDownloadGroupEmployeeHandler(IEmployeeRepository EmployeeRepository)
        {
            _EmployeeRepository = EmployeeRepository;
        }

        public async Task<BulkDownloadGroupEmployeeResponse>  Handle(BulkDownloadGroupEmployeeRequest request, CancellationToken cancellationToken)
        {
          return await _EmployeeRepository.BulkRequestGroupDownload(request, cancellationToken);
        }
    }
}
