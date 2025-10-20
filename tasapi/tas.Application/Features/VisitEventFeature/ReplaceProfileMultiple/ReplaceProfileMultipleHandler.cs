using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple
{
    public sealed class ReplaceProfileMultipleHandler : IRequestHandler<ReplaceProfileMultipleRequest, List<ReplaceProfileMultipleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVisitEventRepository _VisitEventRepository;
        private readonly IMapper _mapper;

        public ReplaceProfileMultipleHandler(IUnitOfWork unitOfWork, IVisitEventRepository VisitEventRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _VisitEventRepository = VisitEventRepository;
            _mapper = mapper;
        }

        public async Task<List<ReplaceProfileMultipleResponse>>  Handle(ReplaceProfileMultipleRequest request, CancellationToken cancellationToken)
        {
            var returnData = await _VisitEventRepository.EventEmployeeReplaceProfileMultiple(request, cancellationToken);
            return returnData;
        }
    }
}
