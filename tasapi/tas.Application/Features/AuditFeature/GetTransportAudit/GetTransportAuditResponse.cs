
namespace tas.Application.Features.AuditFeature.GetTransportAudit
{ 
    public sealed record GetTransportAuditResponse
    {
        public byte[] ExcelFile { get; set; }

    }
}