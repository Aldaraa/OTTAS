
namespace tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCode
{ 
    public sealed record BulkUploadPreviewCostCodeResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<CostCodeBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record CostCodeBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}