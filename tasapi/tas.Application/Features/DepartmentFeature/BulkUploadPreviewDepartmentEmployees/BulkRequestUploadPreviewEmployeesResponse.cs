
namespace tas.Application.Features.DepartmenteFeature.BulkUploadPreviewDepartmentEmployees
{ 
    public sealed record BulkUploadPreviewDepartmentEmployeesResponse
    {

        public int? NoneRow { get; set; }
        public int? UpdateRow { get; set; }
        public List<DepartmentEmployeesBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record DepartmentEmployeesBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}