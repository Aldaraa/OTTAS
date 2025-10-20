
namespace tas.Application.Features.AuditFeature.GetGroupMembersAudit
{
    public sealed record GetGroupMembersAuditResponse
    {
        public int Id { get; set; }

        public string?  Description {get;set;}

        public string?  Action {get;set;}

        public DateTime? CreatedDate  {get;set;}

        public string? ChangedUser  {get;set;}






    }
}