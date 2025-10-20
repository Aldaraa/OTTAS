
namespace tas.Application.Features.PositionFeature.BulkDownloadPositionEmployees
{ 
    public sealed record BulkDownloadPositionEmployeesResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}