using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.RemoveTransport
{

    public sealed class RemoveTransportHandler : IRequestHandler<RemoveTransportRequest, Unit>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RemoveTransportRequest> _validator;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;
        public RemoveTransportHandler(ITransportRepository TransportRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<RemoveTransportRequest> validator, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit> Handle(RemoveTransportRequest request, CancellationToken cancellationToken)
        {
          //  var validationResult = await _validator.ValidateAsync(request);

           var data = await _ITransportRepository.RemoveSchedule(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            if (data != null) {
                if (data.StartScheduleId.HasValue) { 
                    await _transportScheduleCalculateRepository.CalculateByScheduleId(data.StartScheduleId.Value, cancellationToken);
                
                }
                if (data.EndScheduleId.HasValue) {
                    await _transportScheduleCalculateRepository.CalculateByScheduleId(data.EndScheduleId.Value, cancellationToken);
                }
                

            }

            return Unit.Value;

        }
    }
}
