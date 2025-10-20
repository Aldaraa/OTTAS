using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.CreateVisitEvent
{
    public sealed class CreateVisitEventHandler : IRequestHandler<CreateVisitEventRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVisitEventRepository _VisitEventRepository;
        private readonly IMapper _mapper;

        public CreateVisitEventHandler(IUnitOfWork unitOfWork, IVisitEventRepository VisitEventRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _VisitEventRepository = VisitEventRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateVisitEventRequest request, CancellationToken cancellationToken)
        {
            await  _VisitEventRepository.CreateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
