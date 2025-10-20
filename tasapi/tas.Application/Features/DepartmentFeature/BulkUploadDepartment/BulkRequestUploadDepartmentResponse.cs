
namespace tas.Application.Features.DepartmentFeature.BulkUploadDepartment
{ 
    public sealed record BulkUploadDepartmentResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<DepartmentBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record DepartmentBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}