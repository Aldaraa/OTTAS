
namespace tas.Application.Features.RoomFeature.BulkUploadPreviewRoom
{ 
    public sealed record BulkUploadPreviewRoomResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<RoomBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record RoomBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}