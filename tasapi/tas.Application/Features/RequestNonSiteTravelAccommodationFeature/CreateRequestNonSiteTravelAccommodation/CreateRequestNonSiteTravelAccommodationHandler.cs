using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.CreateRequestNonSiteTravelAccommodation
{
    public sealed class CreateRequestNonSiteTravelAccommodationHandler : IRequestHandler<CreateRequestNonSiteTravelAccommodationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelAccommodationRepository _IRequestNonSiteTravelAccommodationRepository;
        private readonly IRequestDocumentRepository _RequestDocRepository;

        public CreateRequestNonSiteTravelAccommodationHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelAccommodationRepository RequestGroupRepository, IMapper mapper, IRequestDocumentRepository requestDocRepository)
        {
            _unitOfWork = unitOfWork;
            _IRequestNonSiteTravelAccommodationRepository = RequestGroupRepository;
            _RequestDocRepository = requestDocRepository;
        }

        public async Task<Unit>  Handle(CreateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {
            await _IRequestNonSiteTravelAccommodationRepository.CreateRequestNonSiteTravelAccommodation(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _RequestDocRepository.GenerateDescription(request.DocumentId, cancellationToken);

            return Unit.Value;
        }
    }
}
