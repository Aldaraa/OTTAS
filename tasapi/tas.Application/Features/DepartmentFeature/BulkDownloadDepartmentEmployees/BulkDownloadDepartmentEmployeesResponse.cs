
namespace tas.Application.Features.DepartmentFeature.BulkDownloadDepartmentEmployees
{ 
    public sealed record BulkDownloadDepartmentEmployeesResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}