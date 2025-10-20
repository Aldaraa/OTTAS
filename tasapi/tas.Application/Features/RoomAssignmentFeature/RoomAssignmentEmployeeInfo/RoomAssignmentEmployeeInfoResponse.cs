
namespace tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo
{ 
    public sealed record RoomAssignmentEmployeeInfoResponse
    {
        public int Id { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public  DateTime? EndDate  { get; set; }
        
        public int? RoomId { get; set; }
        

    }
}