using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelRemove;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public  interface IRequestExternalTravelRemoveRepository : IBaseRepository<RequestExternalTravelRemove>
    {
        Task<int> CreateRequestDocumentExternalTravelRemove(CreateRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentExternalTravelRemoveResponse> GetRequestDocumentExternalTravelRemove(GetRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken);

        Task<int> CompleteRequestDocumentExternalTravelRemove(CompleteRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken);




    }
}
