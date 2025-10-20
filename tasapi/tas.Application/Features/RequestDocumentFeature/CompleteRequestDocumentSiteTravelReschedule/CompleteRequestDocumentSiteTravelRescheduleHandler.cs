using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule
{ 
    public sealed class CompleteRequestDocumentSiteTravelRescheduleHandler : IRequestHandler<CompleteRequestDocumentSiteTravelRescheduleRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRescheduleRepository _RequestSiteTravelRescheduleRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        private readonly IMapper _mapper;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public CompleteRequestDocumentSiteTravelRescheduleHandler(IUnitOfWork unitOfWork,IRequestSiteTravelRescheduleRepository requestSiteTravelRescheduleRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRescheduleRepository = requestSiteTravelRescheduleRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }


        public async Task<Unit> Handle(CompleteRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            await _requestDocumentRepository.DocumentEmployeeActiveCheck(request.Id);
           var  data = await _RequestSiteTravelRescheduleRepository.CompleteRequestDocumentSiteTravelReschedule(request, cancellationToken);
             await _unitOfWork.Save(cancellationToken);

            if(data != null)
            {
                if (data.OldScheduleId.HasValue) {
                  await  _transportScheduleCalculateRepository.CalculateByScheduleId(data.OldScheduleId.Value, cancellationToken);
                }
                if (data.NewScheduleId.HasValue)
                {
                    await _transportScheduleCalculateRepository.CalculateByScheduleId(data.NewScheduleId.Value, cancellationToken);
                }
            }



            return Unit.Value;

        }
    }
}
