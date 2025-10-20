
namespace tas.Application.Features.EmployerFeature.BulkDownloadEmployerEmployees
{ 
    public sealed record BulkDownloadEmployerEmployeesResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}