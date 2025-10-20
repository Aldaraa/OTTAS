using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.BulkDownloadPosition
{
    public sealed class BulkDownloadPositionHandler : IRequestHandler<BulkDownloadPositionRequest, BulkDownloadPositionResponse>
    {
        private readonly IPositionRepository _PositionRepository;

        public BulkDownloadPositionHandler(IPositionRepository PositionRepository)
        {
            _PositionRepository = PositionRepository;
        }

        public async Task<BulkDownloadPositionResponse>  Handle(BulkDownloadPositionRequest request, CancellationToken cancellationToken)
        {
          return await _PositionRepository.BulkRequestDownload(request, cancellationToken);
        }
    }
}
