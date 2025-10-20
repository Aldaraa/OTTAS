using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CompleteRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CreateRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.GetRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestDeMobilisationRepository : IBaseRepository<RequestDeMobilisation>
    {
        Task<int> CreateRequestDocumentDeMobilisation(CreateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken);

        Task CompleteRequestDocumentDeMobilisation(CompleteRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken);


        Task UpdateRequestDocumentDeMobilisation(UpdateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentDeMobilisationResponse> GetRequestDocumentDeMobilisation(GetRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken);
    }
}
