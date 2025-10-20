
namespace tas.Application.Features.PositioneFeature.BulkUploadPreviewPosition
{ 
    public sealed record BulkRequestUploadPreviewPositionResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<PositionBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record PositionBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}