using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode;
using tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode;
using tas.Application.Features.DepartmentGroupConfigFeature.CreateDepartmentGroupConfig;
using tas.Application.Features.DepartmentGroupConfigFeature.DeleteDepartmentGroupConfig;
using tas.Application.Features.DepartmentGroupConfigFeature.GetDepartmentGroupConfig;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface IDepartmentGroupConfigRepository : IBaseRepository<DepartmentGroupConfig>
    {
        Task CreateDepartmentGroupConfig(CreateDepartmentGroupConfigRequest request, CancellationToken cancellationToken);

        Task DeleteDepartmentGroupConfig(DeleteDepartmentGroupConfigRequest request, CancellationToken cancellationToken);

        Task<List<GetDepartmentGroupConfigResponse>> GetDepartmentGroupConfig(GetDepartmentGroupConfigRequest request, CancellationToken cancellationToken);




    }

}
