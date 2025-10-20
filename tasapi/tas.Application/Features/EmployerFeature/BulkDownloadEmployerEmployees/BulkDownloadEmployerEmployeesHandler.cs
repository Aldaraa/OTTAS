using AutoMapper;
using MediatR;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployerEmployees;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.BulkDownloadEmployerEemployees
{
    public sealed class BulkDownloadEmployerEmployeesHandler : IRequestHandler<BulkDownloadEmployerEmployeesRequest, BulkDownloadEmployerEmployeesResponse>
    {
        private readonly IEmployerRepository _EmployerRepository;

        public BulkDownloadEmployerEmployeesHandler(IEmployerRepository employerRepository)
        {
            _EmployerRepository = employerRepository;
        }

        public async Task<BulkDownloadEmployerEmployeesResponse>  Handle(BulkDownloadEmployerEmployeesRequest request, CancellationToken cancellationToken)
        {
          return await _EmployerRepository.BulkRequestEmployeesDownload(request, cancellationToken);
        }
    }
}
