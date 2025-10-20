
namespace tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployerEmployees
{ 
    public sealed record BulkEmployerUploadPreviewEmployeesResponse
    {

        public int? NoneRow { get; set; }

        public int? UpdateRow { get; set; }
        
        public List<EmployerBulkFailedRowEmployees> FailedRows { get; set; }




            
    }

    public sealed record EmployerBulkFailedRowEmployees
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}