using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelReschedule;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IRequestSiteTravelRescheduleRepository : IBaseRepository<RequestSiteTravelReschedule>
    {
        Task<int> CreateRequestDocumentSiteTravelReschedule(CreateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken);

        
        Task<CompleteRequestDocumentSiteTravelRescheduleResponse> CompleteRequestDocumentSiteTravelReschedule(CompleteRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken);


        Task<int> UpdateRequestDocumentSiteTravelReschedule(UpdateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken);

        Task<GetRequestDocumentSiteTravelRescheduleResponse> GetRequestDocumentSiteTravelReschedule(GetRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken);


        Task<List<CheckRequestDocumentSiteTravelRescheduleResponse>> CheckRequestDocumentSiteTravelReschedule(CheckRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken);

    }
}
