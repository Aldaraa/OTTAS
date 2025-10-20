using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterFeature.GetAllRoster;
using tas.Application.Features.SafeModeTransportFeature.CreateTransport;
using tas.Application.Features.SafeModeTransportFeature.DeleteTransport;
using tas.Application.Features.SafeModeTransportFeature.UpdateTransport;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ISafeModeTransportRepository : IBaseRepository<Transport>
    {
        Task CreateTransport(CreateTransportRequest request, CancellationToken cancellationToken);
        Task<int?> DeleteTransport(DeleteTransportRequest request, CancellationToken cancellationToken);

        Task<int?> UpdateTransport(UpdateTransportRequest request, CancellationToken cancellationToken);



    }
}
