using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment
{ 

    public sealed class DownloadRequestDocumentAttachmentHandler : IRequestHandler<DownloadRequestDocumentAttachmentRequest, DownloadRequestDocumentAttachmentResponse>
    {
        private readonly IRequestDocumentAttachmentRepository _IRequestDocumentAttachmentRepository;
        private readonly IMapper _mapper;

        public DownloadRequestDocumentAttachmentHandler(IRequestDocumentAttachmentRepository RequestDocumentAttachmentRepository, IMapper mapper)
        {
            _IRequestDocumentAttachmentRepository = RequestDocumentAttachmentRepository;
            _mapper = mapper;
        }

        public async Task<DownloadRequestDocumentAttachmentResponse> Handle(DownloadRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var data = await _IRequestDocumentAttachmentRepository.DownloadRequestDocumentAttachment(request, cancellationToken);
            return data;
        }
    }
}
