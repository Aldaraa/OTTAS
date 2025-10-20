using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileUndo
{
    public sealed class ReplaceProfileUndoHandler : IRequestHandler<ReplaceProfileUndoRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVisitEventRepository _VisitEventRepository;
        private readonly IMapper _mapper;

        public ReplaceProfileUndoHandler(IUnitOfWork unitOfWork, IVisitEventRepository VisitEventRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _VisitEventRepository = VisitEventRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(ReplaceProfileUndoRequest request, CancellationToken cancellationToken)
        {
            await  _VisitEventRepository.EventEmployeeReplaceProfileUndo(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
