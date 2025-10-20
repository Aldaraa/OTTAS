using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.CreateJob
{

    public sealed class CreateJobHandler : IRequestHandler<CreateJobRequest, int>
    {
        private readonly IMailSmtpConfigRepository _MailSmtpConfigRepository;
        private readonly IMapper _mapper;

        public CreateJobHandler(IMailSmtpConfigRepository MailSmtpConfigRepository)
        {
            _MailSmtpConfigRepository = MailSmtpConfigRepository;
        }

        public async Task<int> Handle(CreateJobRequest request, CancellationToken cancellationToken)
        {
            return await _MailSmtpConfigRepository.CreateTestJob(request, cancellationToken);
        }
    }
}
