using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDelegateFeature.DeleteRequestDelegate
{

    public sealed class DeleteRequestDelegateHandler : IRequestHandler<DeleteRequestDelegateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDelegateRepository _RequestDelegateRepository;
        private readonly IMapper _mapper;

        public DeleteRequestDelegateHandler(IUnitOfWork unitOfWork, IRequestDelegateRepository RequestDelegateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDelegateRepository = RequestDelegateRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestDelegateRequest request, CancellationToken cancellationToken)
        {
            await  _RequestDelegateRepository.DeleteData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
