using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelEmployee;
using tas.Application.Features.RequestDocumentFeature.WaitingAgentRequestDocumentNonSiteTravel;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface  IRequestDocumentNonSiteTravelRepository : IBaseRepository<RequestDocument>
    {
        Task<int> CreateRequestDocumentNonSiteTravel(CreateRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken);
        Task CompleteRequestDocumentNonSiteTravel(CompleteRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken);

        Task WaitingAgentRequestDocumentNonSiteTravel(WaitingAgentRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken);

        void CreateRequestDocumentNonSiteTravelValidate(CreateRequestDocumentNonSiteTravelRequest request);

        Task UpdateRequestDocumentNonSiteTravelEmployee(UpdateRequestDocumentNonSiteTravelEmployeeRequest request, CancellationToken cancellationToken);


        Task UpdateRequestDocumentNonSiteTravelData(UpdateRequestDocumentNonSiteTravelDataRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentNonSiteTravelResponse> GetRequestDocumentNonSiteTravel(GetRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken);


    }
}
