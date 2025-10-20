using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelRemove
{ 
    public sealed class CompleteRequestDocumentExternalTravelRemoveHandler : IRequestHandler<CompleteRequestDocumentExternalTravelRemoveRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelRemoveRepository _RequestExternalTravelRemoveRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public CompleteRequestDocumentExternalTravelRemoveHandler(IUnitOfWork unitOfWork, IRequestExternalTravelRemoveRepository requestExternalTravelRemoveRepository,  IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelRemoveRepository = requestExternalTravelRemoveRepository;
            _mapper = mapper;
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(CompleteRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            await _RequestExternalTravelRemoveRepository.CompleteRequestDocumentExternalTravelRemove(request, cancellationToken);
            return Unit.Value;

        }
    }
}
