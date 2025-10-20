using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.BulkDownloadDepartment
{
    public sealed class BulkDownloadDepartmentHandler : IRequestHandler<BulkDownloadDepartmentRequest, BulkDownloadDepartmentResponse>
    {
        private readonly IDepartmentRepository _DepartmentRepository;

        public BulkDownloadDepartmentHandler(IDepartmentRepository DepartmentRepository)
        {
            _DepartmentRepository = DepartmentRepository;
        }

        public async Task<BulkDownloadDepartmentResponse>  Handle(BulkDownloadDepartmentRequest request, CancellationToken cancellationToken)
        {
          return await _DepartmentRepository.BulkRequestDownload(request, cancellationToken);
        }
    }
}
