using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.StateFeature.DeleteState
{

    public sealed class DeleteStateHandler : IRequestHandler<DeleteStateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStateRepository _StateRepository;
        private readonly IMapper _mapper;

        public DeleteStateHandler(IUnitOfWork unitOfWork, IStateRepository StateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _StateRepository = StateRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteStateRequest request, CancellationToken cancellationToken)
        {
            var State = _mapper.Map<State>(request);
            _StateRepository.Delete(State);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
