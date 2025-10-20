
namespace tas.Application.Features.EmployerFeature.BulkDownloadEmployer
{ 
    public sealed record BulkDownloadEmployerResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}