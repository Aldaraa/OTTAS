using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.MailSmtpConfigFeature.CreateMailSmtpConfig
{
    public sealed class CreateMailSmtpConfigHandler : IRequestHandler<CreateMailSmtpConfigRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailSmtpConfigRepository _MailSmtpConfigRepository;
        private readonly IMapper _mapper;

        public CreateMailSmtpConfigHandler(IUnitOfWork unitOfWork, IMailSmtpConfigRepository MailSmtpConfigRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _MailSmtpConfigRepository = MailSmtpConfigRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateMailSmtpConfigRequest request, CancellationToken cancellationToken)
        {
             await  _MailSmtpConfigRepository.CreateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
