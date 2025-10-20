using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.StateFeature.GetAllState;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IStateRepository : IBaseRepository<State>
    {
        Task<State> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllStateResponse>> GetAllData(GetAllStateRequest request, CancellationToken cancellation);
    }
}
