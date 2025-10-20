
namespace tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees
{ 
    public sealed record BulkDownloadCostCodeEmployeesResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}