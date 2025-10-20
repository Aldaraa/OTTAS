using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelRemove;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    internal class IRequestSiteTravelRemovescheduleRepository
    {
    }


    public interface IRequestSiteTravelRemoveRepository : IBaseRepository<RequestSiteTravelRemove>
    {
        Task<int> CreateRequestDocumentSiteTravelRemove(CreateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken);
        Task<CompleteRequestDocumentSiteTravelRemoveResponse> CompleteRequestDocumentSiteTravelRemove(CompleteRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken);
        Task<int> UpdateRequestDocumentSiteTravelRemove(UpdateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentSiteTravelRemoveResponse> GetRequestDocumentSiteTravelRemove(GetRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken);
        Task<List<CheckRequestDocumentSiteTravelRemoveResponse>> CheckRequestDocumentSiteTravelRemove(CheckRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken);
    }
}
