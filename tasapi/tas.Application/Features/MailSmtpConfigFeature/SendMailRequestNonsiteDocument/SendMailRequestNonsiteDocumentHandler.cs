using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument
{
    public sealed class SendMailRequestNonsiteDocumentHandler : IRequestHandler<SendMailRequestNonsiteDocumentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailSmtpConfigRepository _MailSmtpConfigRepository;
        private readonly IMapper _mapper;

        public SendMailRequestNonsiteDocumentHandler(IUnitOfWork unitOfWork, IMailSmtpConfigRepository MailSmtpConfigRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _MailSmtpConfigRepository = MailSmtpConfigRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(SendMailRequestNonsiteDocumentRequest request, CancellationToken cancellationToken)
        {
             await  _MailSmtpConfigRepository.SendMailNonSiteDocumentNotification(request, cancellationToken);
             await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
