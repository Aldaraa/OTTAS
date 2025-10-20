
namespace tas.Application.Features.CostCodeFeature.BulkDownloadCostCode
{ 
    public sealed record BulkDownloadCostCodeResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}