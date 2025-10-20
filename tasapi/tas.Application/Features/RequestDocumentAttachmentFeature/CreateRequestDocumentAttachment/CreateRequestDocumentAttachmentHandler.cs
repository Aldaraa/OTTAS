using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentAttachmentFeature.CreateRequestDocumentAttachment
{
    public sealed class CreateRequestDocumentAttachmentHandler : IRequestHandler<CreateRequestDocumentAttachmentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentAttachmentRepository _IRequestDocumentAttachmentRepository;

        public CreateRequestDocumentAttachmentHandler(IUnitOfWork unitOfWork, IRequestDocumentAttachmentRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestDocumentAttachmentRepository = RequestGroupRepository;
        }

        public async Task<Unit>  Handle(CreateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            await _IRequestDocumentAttachmentRepository.CreateRequestDocumentAttachment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
