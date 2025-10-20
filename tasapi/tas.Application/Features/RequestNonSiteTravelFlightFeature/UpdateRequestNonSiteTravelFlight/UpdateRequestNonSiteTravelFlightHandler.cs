using AutoMapper;
using MediatR;
using System.Net.NetworkInformation;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelFlightFeature.UpdateRequestNonSiteTravelFlight
{
    public sealed class UpdateRequestNonSiteTravelFlightHandler : IRequestHandler<UpdateRequestNonSiteTravelFlightRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelFlightRepository _IRequestNonSiteTravelFlightRepository;
        private readonly IRequestDocumentRepository _IRequestDocumentRepository;

        public UpdateRequestNonSiteTravelFlightHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelFlightRepository RequestGroupRepository, IMapper mapper, IRequestDocumentRepository iRequestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _IRequestNonSiteTravelFlightRepository = RequestGroupRepository;
            _IRequestDocumentRepository = iRequestDocumentRepository;
        }

        public async Task<Unit>  Handle(UpdateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {
            await _IRequestNonSiteTravelFlightRepository.UpdateRequestNonSiteTravelFlight(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
           await _IRequestDocumentRepository.GenerateDescription(request.DocumentId, cancellationToken);

            return Unit.Value;
        }
    }
}
