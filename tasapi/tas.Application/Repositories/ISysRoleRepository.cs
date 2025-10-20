using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Application.Features.SysRoleFeature.AddEmployee;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;
using tas.Application.Features.SysRoleFeature.GetEmployeeRoleInfo;
using tas.Application.Features.SysRoleFeature.GetSysRole;
using tas.Application.Features.SysRoleFeature.RemoveEmployeeRole;
using tas.Application.Features.SysRoleFeature.UpdateReadOnlyAccesss;
using tas.Application.Features.SysTeamFeature.GetSysTeam;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ISysRoleRepository : IBaseRepository<SysRole>
    {
        Task<List<GetAllSysRoleResponse>> GetAllData(CancellationToken cancellationToken);

        Task<GetSysRoleResponse> GetData(GetSysRoleRequest request, CancellationToken cancellationToken);

        Task AddEmployee(AddEmployeeRequest request, CancellationToken cancellationToken);


        Task RemoveEmployee(RemoveEmployeeRoleRequest request, CancellationToken cancellationToken);

        Task UpdateReadOnlyAccesss(UpdateReadOnlyAccesssRequest request, CancellationToken cancellationToken);

        Task<GetEmployeeRoleInfoResponse> GetRoleInfoData(GetEmployeeRoleInfoRequest request, CancellationToken cancellationToken);


    }
}
