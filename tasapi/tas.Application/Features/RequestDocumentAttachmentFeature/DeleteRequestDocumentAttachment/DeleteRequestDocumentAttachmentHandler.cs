using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.DeleteRequestDocumentAttachment
{
    public sealed class DeleteRequestDocumentAttachmentHandler : IRequestHandler<DeleteRequestDocumentAttachmentRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentAttachmentRepository _IRequestDocumentAttachmentRepository;

        public DeleteRequestDocumentAttachmentHandler(IUnitOfWork unitOfWork, IRequestDocumentAttachmentRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestDocumentAttachmentRepository = RequestGroupRepository;
        }

        public async Task<int> Handle(DeleteRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
          var documentId =  await _IRequestDocumentAttachmentRepository.DeleteRequestDocumentAttachment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return documentId;
        }
    }
}
