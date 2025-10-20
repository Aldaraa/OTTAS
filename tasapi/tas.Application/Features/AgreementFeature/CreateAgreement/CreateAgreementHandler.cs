using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.AgreementFeature.CreateAgreement
{
    public sealed class CreateAgreementHandler : IRequestHandler<CreateAgreementRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAgreementRepository _AgreementRepository;
        private readonly IMapper _mapper;

        public CreateAgreementHandler(IUnitOfWork unitOfWork, IAgreementRepository AgreementRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _AgreementRepository = AgreementRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateAgreementRequest request, CancellationToken cancellationToken)
        {
             await  _AgreementRepository.CreateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
