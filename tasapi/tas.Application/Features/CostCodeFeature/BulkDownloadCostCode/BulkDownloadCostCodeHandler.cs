using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.BulkDownloadCostCode
{
    public sealed class BulkDownloadCostCodeHandler : IRequestHandler<BulkDownloadCostCodeRequest, BulkDownloadCostCodeResponse>
    {
        private readonly ICostCodeRepository _CostCodeRepository;

        public BulkDownloadCostCodeHandler(ICostCodeRepository costCodeRepository)
        {
            _CostCodeRepository = costCodeRepository;
        }

        public async Task<BulkDownloadCostCodeResponse>  Handle(BulkDownloadCostCodeRequest request, CancellationToken cancellationToken)
        {
          return await _CostCodeRepository.BulkRequestDownload(request, cancellationToken);
        }
    }
}
