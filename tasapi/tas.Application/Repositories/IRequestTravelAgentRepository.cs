using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestTravelAgentRepository : IBaseRepository<RequestTravelAgent>
    {
        Task<List<GetAllRequestTravelAgentResponse>> GetAllData(GetAllRequestTravelAgentRequest request, CancellationToken cancellationToken);
    }
}
