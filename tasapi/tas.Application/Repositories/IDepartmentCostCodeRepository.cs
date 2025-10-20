using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode;
using tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IDepartmentCostCodeRepository : IBaseRepository<DepartmentCostCode>
    {

        Task DeleteDepartmentCostCode(DeleteDepartmentCostCodeRequest request, CancellationToken cancellationToken);

        Task AddDepartmentCostCode(AddDepartmentCostCodeRequest request, CancellationToken cancellationToken);
    }

}
