using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentAttachmentFeature.CreateRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.DeleteRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.UpdateRequestDocumentAttachment;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestDocumentAttachmentRepository : IBaseRepository<RequestDocumentAttachment>
    {
        Task<List<GetRequestDocumentAttachmentResponse>> GetRequestDocumentAttachment(GetRequestDocumentAttachmentRequest request, CancellationToken cancellationToken);

        Task  CreateRequestDocumentAttachment(CreateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken);

        Task<int>  UpdateRequestDocumentAttachment(UpdateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken);


        Task<int> DeleteRequestDocumentAttachment(DeleteRequestDocumentAttachmentRequest request, CancellationToken cancellationToken);

        Task<DownloadRequestDocumentAttachmentResponse> DownloadRequestDocumentAttachment(DownloadRequestDocumentAttachmentRequest request, CancellationToken cancellationToken);




    }
}
