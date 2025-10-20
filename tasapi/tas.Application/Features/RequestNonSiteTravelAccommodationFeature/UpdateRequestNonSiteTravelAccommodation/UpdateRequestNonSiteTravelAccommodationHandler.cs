using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.UpdateRequestNonSiteTravelAccommodation
{
    public sealed class UpdateRequestNonSiteTravelAccommodationHandler : IRequestHandler<UpdateRequestNonSiteTravelAccommodationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelAccommodationRepository _IRequestNonSiteTravelAccommodationRepository;
        private readonly IRequestDocumentRepository _RequestDocRepository;

        public UpdateRequestNonSiteTravelAccommodationHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelAccommodationRepository RequestGroupRepository, IMapper mapper, IRequestDocumentRepository requestDocRepository)
        {
            _unitOfWork = unitOfWork;
            _IRequestNonSiteTravelAccommodationRepository = RequestGroupRepository;
            _RequestDocRepository = requestDocRepository;
        }

        public async Task<Unit>  Handle(UpdateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {
          var docId =  await _IRequestNonSiteTravelAccommodationRepository.UpdateRequestNonSiteTravelAccommodation(request, cancellationToken);
            if (docId > 0) {
                await _RequestDocRepository.GenerateDescription(docId, cancellationToken);
            }
            
            return Unit.Value;
        }
    }
}
