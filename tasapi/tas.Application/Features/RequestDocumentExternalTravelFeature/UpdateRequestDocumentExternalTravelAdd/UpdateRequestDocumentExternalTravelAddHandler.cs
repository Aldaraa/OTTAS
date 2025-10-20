using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelAdd
{ 
    public sealed class UpdateRequestDocumentExternalTravelAddHandler : IRequestHandler<UpdateRequestDocumentExternalTravelAddRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelAddRepository _RequestExternalTravelAddRepository;
        private readonly IMapper _mapper;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        private readonly ITransportCheckerRepository _transportCheckerRepository;


        public UpdateRequestDocumentExternalTravelAddHandler(IUnitOfWork unitOfWork, IRequestExternalTravelAddRepository requestExternalTravelAddRepository, IRequestDocumentRepository requestDocumentRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelAddRepository = requestExternalTravelAddRepository;
            _requestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;
        }


        public async Task<Unit> Handle(UpdateRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            await _RequestExternalTravelAddRepository.UpdateRequestDocumentExternalTravelAdd(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _requestDocumentRepository.GenerateDescription(request.Id, cancellationToken);
            return Unit.Value;

        }
    }
}
