using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PeopleTypeFeature.GetAllPeopleType;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IPeopleTypeRepository : IBaseRepository<PeopleType>
    {
        Task<PeopleType> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllPeopleTypeResponse>> GetAllData(GetAllPeopleTypeRequest request, CancellationToken cancellationToken);
    }
}
