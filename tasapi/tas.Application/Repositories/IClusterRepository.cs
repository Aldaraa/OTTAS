using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.ClusterFeature.GetActiveTransportCluster;
using tas.Application.Features.ClusterFeature.GetAllCluster;
using tas.Application.Features.ClusterFeature.GetCluster;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IClusterRepository : IBaseRepository<Cluster>
    {
        Task <List<GetActiveTransportClusterResponse>> GetActiveTranports(GetActiveTransportClusterRequest request, CancellationToken cancellationToken);


        Task<GetClusterResponse> GetCluster(GetClusterRequest request, CancellationToken cancellationToken);


        Task<List<GetAllClusterResponse>> GetAllData(GetAllClusterRequest request, CancellationToken cancellationToken);
    }
}
