
namespace tas.Application.Features.AuditFeature.GetEmployeeAudit
{ 


    public sealed record GetEmployeeAuditResponse
    {
        public DateTime? CreatedDate { get; set; }

        public string? CreatedFullName { get; set; }

        public int? CreatedUserId { get; set; }

        public List<GetEmployeeAuditResponseData> AuditData { get; set; }


    }


    public sealed record GetEmployeeAuditResponseData
    {
        public int? Id { get; set; }

        public string? Username { get; set; }

        public int? UserId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime? DateCreated { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }
    }
}