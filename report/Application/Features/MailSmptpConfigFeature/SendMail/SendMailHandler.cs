using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.MailSmptpConfigFeature.SendMail
{

    public sealed class SendMailHandler : IRequestHandler<SendMailRequest, int>
    {
        private readonly IMailSmtpConfigRepository _MailSmptpConfigRepository;
        private readonly IMapper _mapper;

        public SendMailHandler(IMailSmtpConfigRepository MailSmptpConfigRepository)
        {
            _MailSmptpConfigRepository = MailSmptpConfigRepository;
        }

        public async Task<int> Handle(SendMailRequest request, CancellationToken cancellationToken)
        {
           return await _MailSmptpConfigRepository.SendMail(request, cancellationToken);
        }
    }
}
