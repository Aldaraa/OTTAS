
namespace tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployer
{ 
    public sealed record BulkEmployerUploadPreviewResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<EmployerBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record EmployerBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}