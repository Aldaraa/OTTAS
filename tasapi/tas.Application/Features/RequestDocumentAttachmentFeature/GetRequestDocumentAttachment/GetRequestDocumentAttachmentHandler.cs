using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment
{ 

    public sealed class GetRequestDocumentAttachmentHandler : IRequestHandler<GetRequestDocumentAttachmentRequest, List<GetRequestDocumentAttachmentResponse>>
    {
        private readonly IRequestDocumentAttachmentRepository _IRequestDocumentAttachmentRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentAttachmentHandler(IRequestDocumentAttachmentRepository RequestDocumentAttachmentRepository, IMapper mapper)
        {
            _IRequestDocumentAttachmentRepository = RequestDocumentAttachmentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestDocumentAttachmentResponse>> Handle(GetRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var data = await _IRequestDocumentAttachmentRepository.GetRequestDocumentAttachment(request, cancellationToken);
            return data;
        }
    }
}
