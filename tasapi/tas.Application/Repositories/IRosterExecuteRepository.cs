using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectute;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectutePreview;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecute;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecutePreview;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRosterExecuteRepository : IBaseRepository<Roster>
    {
        Task<BulkRosterExecuteResponse> ExecuteBulkRoster(BulkRosterExecuteRequest request, CancellationToken cancellationToken);

        Task<List<BulkRosterExecutePreviewResponse>> ExecuteBulkRosterPreview(BulkRosterExecutePreviewRequest request, CancellationToken cancellationToken);


    }
}
