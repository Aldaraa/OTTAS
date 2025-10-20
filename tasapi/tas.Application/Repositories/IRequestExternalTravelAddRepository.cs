using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelAdd;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestExternalTravelAddRepository : IBaseRepository<RequestExternalTravelAdd>
    {
        Task<int> CreateRequestExternalTravelAdd(CreateRequestExternalTravelAddRequest request, CancellationToken cancellationToken);
        Task UpdateRequestDocumentExternalTravelAdd(UpdateRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken);

          Task<GetRequestDocumentExternalTravelAddResponse>   GetRequestDocumentExternalTravelAdd(GetRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken);

          Task CompleteRequestDocumentExternalTravelAdd(CompleteRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken);



    }
}



