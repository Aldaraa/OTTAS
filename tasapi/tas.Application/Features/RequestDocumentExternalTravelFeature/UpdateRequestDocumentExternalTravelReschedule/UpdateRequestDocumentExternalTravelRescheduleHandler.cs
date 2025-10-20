using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelReschedule
{ 
    public sealed class UpdateRequestDocumentExternalTravelRescheduleHandler : IRequestHandler<UpdateRequestDocumentExternalTravelRescheduleRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelReScheduleRepository _RequestExternalTravelRescheduleRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDocumentExternalTravelRescheduleHandler(IUnitOfWork unitOfWork, IRequestExternalTravelReScheduleRepository requestExternalTravelRescheduleRepository,  IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelRescheduleRepository = requestExternalTravelRescheduleRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(UpdateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            await _RequestExternalTravelRescheduleRepository.UpdateRequestDocumentExternalTravelReschedule(request, cancellationToken);
            return Unit.Value;

        }
    }
}
