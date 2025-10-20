using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ICheckDataRepository : IBaseRepository<Bed>
    {
        Task<bool> CheckProfile(int EmployeeId, CancellationToken cancellationToken);
    }


}