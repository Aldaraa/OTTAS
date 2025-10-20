using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelReschedule;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestExternalTravelReScheduleRepository : IBaseRepository<RequestExternalTravelReschedule>
    {
        Task<int> CreateRequestDocumentExternalTravelReschedule(CreateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken);

        Task UpdateRequestDocumentExternalTravelReschedule(UpdateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken);
        Task<GetRequestDocumentExternalTravelRescheduleResponse> GetRequestDocumentExternalTravelReschedule(GetRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken);

        Task CompleteRequestDocumentExternalTravelReschedule(CompleteRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken);




    }
}
