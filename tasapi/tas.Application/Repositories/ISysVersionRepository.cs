using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysVersionFeature.CreateSysVersion;
using tas.Application.Features.SysVersionFeature.GetSysVersion;
using tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory;
using tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ISysVersionRepository : IBaseRepository<SysVersion>
    {

        Task CreateVersion(CreateSysVersionRequest request, CancellationToken cancellationToken);

        Task<GetSysVersionResponse>  GeVersion(GetSysVersionRequest request, CancellationToken cancellationToken);

        Task<GetSysVersionNoteResponse> GeVersionNote(GetSysVersionNoteRequest request, CancellationToken cancellationToken);

        Task<List<GetSysVersionHistoryResponse>> GeVersionHistory(GetSysVersionHistoryRequest request, CancellationToken cancellationToken);



    }
}
