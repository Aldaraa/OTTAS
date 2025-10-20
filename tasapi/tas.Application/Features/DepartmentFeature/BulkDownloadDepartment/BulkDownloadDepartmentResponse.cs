
namespace tas.Application.Features.DepartmentFeature.BulkDownloadDepartment
{ 
    public sealed record BulkDownloadDepartmentResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}