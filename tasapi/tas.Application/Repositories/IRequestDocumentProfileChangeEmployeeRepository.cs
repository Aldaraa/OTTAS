using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChangeTemp;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface  IRequestDocumentProfileChangeEmployeeRepository : IBaseRepository<RequestDocumentProfileChangeEmployee>
    {
        Task<int> CreateRequestDocumentProfileChange(CreateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken);

        Task<int> CreateRequestDocumentProfileChangeTemp(CreateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken);


        Task UpdateRequestDocumentProfileChange(UpdateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken);

        Task UpdateRequestDocumentProfileChangeTemp(UpdateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken);



        Task CompleteRequestDocumentProfileChange(CompleteRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken);

        Task CompleteRequestDocumentProfileChangeTemp(CompleteRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken);


        Task<GetRequestDocumentProfileChangeResponse> GetRequestDocumentProfile(GetRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken);
        Task<GetRequestDocumentProfileChangeTempResponse> GetRequestDocumentProfileTemp(GetRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken);

    }
}
