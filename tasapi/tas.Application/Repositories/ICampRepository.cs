using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ICampRepository : IBaseRepository<Camp>
    {
        Task<Camp> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllCampResponse>> GetAllData(GetAllCampRequest request, CancellationToken cancellationToken);
    }
}
