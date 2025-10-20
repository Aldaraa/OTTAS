
namespace tas.Application.Features.RoomFeature.BulkDownloadRoom
{ 
    public sealed record BulkDownloadRoomResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}