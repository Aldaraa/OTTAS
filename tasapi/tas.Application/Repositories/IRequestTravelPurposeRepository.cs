using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelPurposeFeature.GetAllRequestTravelPurpose;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestTravelPurposeRepository : IBaseRepository<RequestTravelPurpose>
    {

        Task<List<GetAllRequestTravelPurposeResponse>> GetAllData(GetAllRequestTravelPurposeRequest request, CancellationToken cancellationToken);

    }
}
