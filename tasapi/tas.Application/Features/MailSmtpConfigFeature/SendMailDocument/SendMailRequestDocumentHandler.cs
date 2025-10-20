using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument
{
    public sealed class SendMailRequestDocumentHandler : IRequestHandler<SendMailRequestDocumentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailSmtpConfigRepository _MailSmtpConfigRepository;
        private readonly IMapper _mapper;

        public SendMailRequestDocumentHandler(IUnitOfWork unitOfWork, IMailSmtpConfigRepository MailSmtpConfigRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _MailSmtpConfigRepository = MailSmtpConfigRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(SendMailRequestDocumentRequest request, CancellationToken cancellationToken)
        {
             await  _MailSmtpConfigRepository.SendMailRequestDocumentNotification(request, cancellationToken);
             await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
