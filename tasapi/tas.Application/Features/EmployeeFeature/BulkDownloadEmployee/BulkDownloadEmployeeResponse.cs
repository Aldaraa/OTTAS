
namespace tas.Application.Features.EmployeeFeature.BulkDownloadEmployee
{ 
    public sealed record BulkDownloadEmployeeResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}