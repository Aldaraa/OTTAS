using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestNonSiteTravelAccommodationFeature.DeleteRequestNonSiteTravelAccommodation
{
    public sealed class DeleteRequestNonSiteTravelAccommodationHandler : IRequestHandler<DeleteRequestNonSiteTravelAccommodationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestNonSiteTravelAccommodationRepository _IRequestNonSiteTravelAccommodationRepository;
        private readonly IRequestDocumentRepository _IRequestDocumentRepository;

        public DeleteRequestNonSiteTravelAccommodationHandler(IUnitOfWork unitOfWork, IRequestNonSiteTravelAccommodationRepository RequestGroupRepository, IMapper mapper, IRequestDocumentRepository iRequestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _IRequestNonSiteTravelAccommodationRepository = RequestGroupRepository;
            _IRequestDocumentRepository = iRequestDocumentRepository;
        }

        public async Task<Unit>  Handle(DeleteRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {
           var docId = await _IRequestNonSiteTravelAccommodationRepository.DeleteRequestNonSiteTravelAccommodation(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            if (docId > 0)
            {
                await _IRequestDocumentRepository.GenerateDescription(docId, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
