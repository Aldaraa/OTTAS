using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface INationalityRepository : IBaseRepository<Nationality>
    {
        Task<Nationality> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllNationalityResponse>> GetAllData(GetAllNationalityRequest request, CancellationToken cancellationToken);
    }
}
