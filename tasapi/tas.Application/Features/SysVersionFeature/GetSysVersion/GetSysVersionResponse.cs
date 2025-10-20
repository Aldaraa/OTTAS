
using tas.Domain.Entities;

namespace tas.Application.Features.SysVersionFeature.GetSysVersion
{ 
    public sealed record GetSysVersionResponse
    {
        public int Id { get; set; }
        public string? Version { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}