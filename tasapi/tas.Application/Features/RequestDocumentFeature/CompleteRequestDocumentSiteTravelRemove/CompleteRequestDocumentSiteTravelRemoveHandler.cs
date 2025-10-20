using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelRemove
{ 
    public sealed class CompleteRequestDocumentSiteTravelRemoveHandler : IRequestHandler<CompleteRequestDocumentSiteTravelRemoveRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRemoveRepository _RequestSiteTravelRemoveRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public CompleteRequestDocumentSiteTravelRemoveHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRemoveRepository requestSiteTravelRemoveRepository,  IMapper mapper, IRequestDocumentRepository requestDocumentRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRemoveRepository = requestSiteTravelRemoveRepository;
            _mapper = mapper;
            _RequestDocumentRepository = requestDocumentRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }


        public async Task<Unit> Handle(CompleteRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
           var removedData = await _RequestSiteTravelRemoveRepository.CompleteRequestDocumentSiteTravelRemove(request, cancellationToken);
            if (removedData != null) {
                if (removedData.StartScheduleId.HasValue) {
                   await _transportScheduleCalculateRepository.CalculateByScheduleId(removedData.StartScheduleId.Value, cancellationToken);
                }
                if (removedData.EndScheduleId.HasValue)
                {
                    await _transportScheduleCalculateRepository.CalculateByScheduleId(removedData.EndScheduleId.Value, cancellationToken);
                }
            }

            return Unit.Value;

        }
    }
}
