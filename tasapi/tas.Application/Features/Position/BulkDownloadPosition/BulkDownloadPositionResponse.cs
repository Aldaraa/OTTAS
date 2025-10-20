
namespace tas.Application.Features.PositionFeature.BulkDownloadPosition
{ 
    public sealed record BulkDownloadPositionResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}