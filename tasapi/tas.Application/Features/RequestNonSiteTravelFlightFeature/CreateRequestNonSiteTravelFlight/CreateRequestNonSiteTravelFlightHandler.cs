using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.CreateRequestNonSiteTravelFlight
{
    public sealed class CreateRequestNonSiteTravelFlightHandler : IRequestHandler<CreateRequestNonSiteTravelFlightRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelFlightRepository _IRequestNonSiteTravelFlightRepository;
        private readonly IRequestDocumentRepository _RequestDocRepository;

        public CreateRequestNonSiteTravelFlightHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelFlightRepository RequestGroupRepository, IMapper mapper, IRequestDocumentRepository requestDocRepository)
        {
            _unitOfWork = unitOfWork;
            _IRequestNonSiteTravelFlightRepository = RequestGroupRepository;
            _RequestDocRepository = requestDocRepository;
        }

        public async Task<Unit>  Handle(CreateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {
            await _IRequestNonSiteTravelFlightRepository.CreateRequestNonSiteTravelFlight(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _RequestDocRepository.GenerateDescription(request.DocumentId, cancellationToken);
            

            return Unit.Value;
        }
    }
}
