using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfile
{
    public sealed class ReplaceProfileHandler : IRequestHandler<ReplaceProfileRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVisitEventRepository _VisitEventRepository;
        private readonly IMapper _mapper;

        public ReplaceProfileHandler(IUnitOfWork unitOfWork, IVisitEventRepository VisitEventRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _VisitEventRepository = VisitEventRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(ReplaceProfileRequest request, CancellationToken cancellationToken)
        {
            await  _VisitEventRepository.EventEmployeeReplaceProfile(request, cancellationToken);
            return Unit.Value;
        }
    }
}
