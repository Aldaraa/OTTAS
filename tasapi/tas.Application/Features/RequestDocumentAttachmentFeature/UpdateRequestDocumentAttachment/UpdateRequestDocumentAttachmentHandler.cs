using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.UpdateRequestDocumentAttachment
{
    public sealed class UpdateRequestDocumentAttachmentHandler : IRequestHandler<UpdateRequestDocumentAttachmentRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentAttachmentRepository _IRequestDocumentAttachmentRepository;

        public UpdateRequestDocumentAttachmentHandler(IUnitOfWork unitOfWork, IRequestDocumentAttachmentRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestDocumentAttachmentRepository = RequestGroupRepository;
        }

        public async Task<int>  Handle(UpdateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var documentId =  await _IRequestDocumentAttachmentRepository.UpdateRequestDocumentAttachment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return documentId;
        }
    }
}
