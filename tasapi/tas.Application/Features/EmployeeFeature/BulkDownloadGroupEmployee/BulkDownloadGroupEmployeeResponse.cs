
namespace tas.Application.Features.EmployeeFeature.BulkDownloadGroupEmployee
{ 
    public sealed record BulkDownloadGroupEmployeeResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}