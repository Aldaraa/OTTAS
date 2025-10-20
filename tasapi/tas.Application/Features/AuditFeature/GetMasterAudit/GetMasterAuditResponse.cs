
namespace tas.Application.Features.AuditFeature.GetMasterAudit
{ 
    public sealed record GetMasterAuditResponse
    {
        public int? Id { get; set; }

        public string? Username { get; set; }

        public int? UserId { get; set; }

        public DateTime? DateCreated { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

    }
}