
using tas.Domain.Entities;

namespace tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory
{ 
    public sealed record GetSysVersionHistoryResponse
    {
        public int Id { get; set; }
        public string? Version { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}