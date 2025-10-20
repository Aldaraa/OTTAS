using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.DeleteRequestNonSiteTravelFlight
{
    public sealed class DeleteRequestNonSiteTravelFlightHandler : IRequestHandler<DeleteRequestNonSiteTravelFlightRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelFlightRepository _IRequestNonSiteTravelFlightRepository;

        public DeleteRequestNonSiteTravelFlightHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelFlightRepository RequestGroupRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _IRequestNonSiteTravelFlightRepository = RequestGroupRepository;
        }

        public async Task<Unit>  Handle(DeleteRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {
            await _IRequestNonSiteTravelFlightRepository.DeleteRequestNonSiteTravelFlight(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
