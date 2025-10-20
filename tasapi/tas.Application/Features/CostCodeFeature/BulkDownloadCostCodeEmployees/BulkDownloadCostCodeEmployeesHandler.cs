using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees
{
    public sealed class BulkDownloadCostCodeEmployeesHandler : IRequestHandler<BulkDownloadCostCodeEmployeesRequest, BulkDownloadCostCodeEmployeesResponse>
    {
        private readonly ICostCodeRepository _CostCodeRepository;

        public BulkDownloadCostCodeEmployeesHandler(ICostCodeRepository costCodeRepository)
        {
            _CostCodeRepository = costCodeRepository;
        }

        public async Task<BulkDownloadCostCodeEmployeesResponse>  Handle(BulkDownloadCostCodeEmployeesRequest request, CancellationToken cancellationToken)
        {
          return await _CostCodeRepository.BulkRequestEmployeeDownload(request, cancellationToken);
        }
    }
}
