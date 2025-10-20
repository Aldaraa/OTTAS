using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.MailSmtpConfigFeature.GetMailSmtpConfig
{

    public sealed class GetMailSmtpConfigHandler : IRequestHandler<GetMailSmtpConfigRequest, GetMailSmtpConfigResponse>
    {
        private readonly IMailSmtpConfigRepository _MailSmtpConfigRepository;
        private readonly IMapper _mapper;

        public GetMailSmtpConfigHandler(IMailSmtpConfigRepository MailSmtpConfigRepository, IMapper mapper)
        {
            _MailSmtpConfigRepository = MailSmtpConfigRepository;
            _mapper = mapper;
        }

        public async Task<GetMailSmtpConfigResponse> Handle(GetMailSmtpConfigRequest request, CancellationToken cancellationToken)
        {
            var returnData = await _MailSmtpConfigRepository.GetData(request, cancellationToken);
            return returnData;


        }
    }
}
