using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.BulkDownloadPositionEmployees
{
    public sealed class BulkDownloadPositionEmployeesHandler : IRequestHandler<BulkDownloadPositionEmployeesRequest, BulkDownloadPositionEmployeesResponse>
    {
        private readonly IPositionRepository _PositionRepository;

        public BulkDownloadPositionEmployeesHandler(IPositionRepository PositionRepository)
        {
            _PositionRepository = PositionRepository;
        }

        public async Task<BulkDownloadPositionEmployeesResponse>  Handle(BulkDownloadPositionEmployeesRequest request, CancellationToken cancellationToken)
        {
          return await _PositionRepository.BulkRequestEmployeeDownload(request, cancellationToken);
        }
    }
}
