using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelAdd
{ 
    public sealed class CompleteRequestDocumentSiteTravelHandler : IRequestHandler<CompleteRequestDocumentSiteTravelAddRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelAddRepository _RequestSiteTravelAddRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;

        public CompleteRequestDocumentSiteTravelHandler(IUnitOfWork unitOfWork, IRequestSiteTravelAddRepository requestSiteTravelAddRepository, IMapper mapper, ITransportRepository transportRepository, IRequestDocumentRepository requestDocumentRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelAddRepository = requestSiteTravelAddRepository;
            _mapper = mapper;
            _TransportRepository = transportRepository;
            _RequestDocumentRepository = requestDocumentRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }


        public async Task<Unit> Handle(CompleteRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            await _RequestDocumentRepository.DocumentEmployeeActiveCompleteAddTravelCheck(request.Id);
            

           var data = await _RequestSiteTravelAddRepository.CompleteRequestDocumentSiteTravelAdd(request, cancellationToken);
            if(data != null)
            {
                if (data.InScheduleId.HasValue) {
                  await  _transportScheduleCalculateRepository.CalculateByScheduleId(data.InScheduleId.Value, cancellationToken);
                }
                if (data.OutScheduleId.HasValue)
                {
                    await _transportScheduleCalculateRepository.CalculateByScheduleId(data.OutScheduleId.Value, cancellationToken);
                }

            }

            return Unit.Value;



        }
    }
}
