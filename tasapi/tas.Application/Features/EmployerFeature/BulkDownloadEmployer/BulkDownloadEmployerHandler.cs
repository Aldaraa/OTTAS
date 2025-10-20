using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.BulkDownloadEmployer
{
    public sealed class BulkDownloadEmployerHandler : IRequestHandler<BulkDownloadEmployerRequest, BulkDownloadEmployerResponse>
    {
        private readonly IEmployerRepository _EmployerRepository;

        public BulkDownloadEmployerHandler(IEmployerRepository employerRepository)
        {
            _EmployerRepository = employerRepository;
        }

        public async Task<BulkDownloadEmployerResponse>  Handle(BulkDownloadEmployerRequest request, CancellationToken cancellationToken)
        {
          return await _EmployerRepository.BulkRequestDownload(request, cancellationToken);
        }
    }
}
