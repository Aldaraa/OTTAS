
namespace tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployeeGroup
{ 
    public sealed record BulkUploadPreviewEmployeeGroupResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<EmployeeGroupBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record EmployeeGroupBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}