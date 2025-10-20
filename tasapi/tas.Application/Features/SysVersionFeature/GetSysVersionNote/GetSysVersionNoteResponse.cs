
using tas.Domain.Entities;

namespace tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote
{ 
    public sealed record GetSysVersionNoteResponse
    {
        public int Id { get; set; }
        public string? Version { get; set; }
        public string? ReleaseNote { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}