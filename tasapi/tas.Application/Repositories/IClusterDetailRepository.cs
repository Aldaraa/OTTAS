using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterDetailFeature.CreateClusterDetail;
using tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail;
using tas.Application.Features.ClusterDetailFeature.ReOrderClusterDetail;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface IClusterDetailRepository : IBaseRepository<ClusterDetail>
    {
        //Task<List<GetActiveTransportClusterResponse>> GetActiveTranports(GetActiveTransportClusterRequest request, CancellationToken cancellationToken);

        Task Create(CreateClusterDetailRequest request, CancellationToken cancellationToken);

        Task ReOrder(ReOrderClusterDetailRequest request, CancellationToken cancellationToken);

        Task Delete(DeleteClusterDetailRequest request, CancellationToken cancellationToken);
    }
}
