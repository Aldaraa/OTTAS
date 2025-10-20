using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate
{
    public sealed class CreateRequestDelegateHandler : IRequestHandler<CreateRequestDelegateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDelegateRepository _RequestDelegateRepository;
        private readonly IMapper _mapper;

        public CreateRequestDelegateHandler(IUnitOfWork unitOfWork, IRequestDelegateRepository RequestDelegateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDelegateRepository = RequestDelegateRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRequestDelegateRequest request, CancellationToken cancellationToken)
        {
            _RequestDelegateRepository.CreateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
