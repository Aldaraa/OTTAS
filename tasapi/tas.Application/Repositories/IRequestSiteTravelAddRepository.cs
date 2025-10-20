using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestSiteTravelAddRepository : IBaseRepository<RequestSiteTravelAdd>
    {
        Task<int> CreateRequestDocumentSiteTravelAdd(CreateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken);
      


        Task<CompleteRequestDocumentSiteTravelAddResponse?> CompleteRequestDocumentSiteTravelAdd(CompleteRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken);
        
        
        Task<int> UpdateRequestDocumentSiteTravelAdd(UpdateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentSiteTravelAddResponse> GetRequestDocumentSiteTravelAdd(GetRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken);

        Task<List<CheckRequestDocumentSiteTravelAddResponse>> CheckRequestDocumentSiteTravelAdd(CheckRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken);


    }
}
