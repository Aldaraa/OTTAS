using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.SysTeamFeature.DeleteUserSysTeam;
using tas.Application.Features.SysTeamFeature.GetAllSysTeam;
using tas.Application.Features.SysTeamFeature.GetSysTeam;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Features.SysTeamFeature.SetUserSysTeam;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ISysTeamRepository : IBaseRepository<SysTeam>
    {
        //GetAllSysTeam
        Task<List<GetAllSysTeamResponse>> GetAllSysTeam(CancellationToken cancellationToken);

        Task<GetSysTeamResponse> GetSysTeamProfile(int teamId, CancellationToken cancellationToken);

        Task SetMenuSysTeam(SetMenuSysTeamBulkRequest requests, CancellationToken cancellationToken);


        Task SetUserSysTeam(SetUserSysTeamRequest requests, CancellationToken cancellationToken);


        Task DeleteUserSysTeam(DeleteUserSysTeamRequest requests, CancellationToken cancellationToken);
    }
}
