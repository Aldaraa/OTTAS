using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.UpdateVisitEvent
{
    public sealed class UpdateVisitEventHandler : IRequestHandler<UpdateVisitEventRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVisitEventRepository _VisitEventRepository;
        private readonly IMapper _mapper;

        public UpdateVisitEventHandler(IUnitOfWork unitOfWork, IVisitEventRepository VisitEventRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _VisitEventRepository = VisitEventRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(UpdateVisitEventRequest request, CancellationToken cancellationToken)
        {
            await  _VisitEventRepository.UpdateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
