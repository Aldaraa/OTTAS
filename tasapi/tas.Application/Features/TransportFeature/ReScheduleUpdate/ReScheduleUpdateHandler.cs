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

namespace tas.Application.Features.TransportFeature.ReScheduleUpdate
{

    public sealed class ReScheduleUpdateHandler : IRequestHandler<ReScheduleUpdateRequest, Unit>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ReScheduleUpdateRequest> _validator;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;

        public ReScheduleUpdateHandler(ITransportRepository TransportRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ReScheduleUpdateRequest> validator, ITransportCheckerRepository transportCheckerRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _transportCheckerRepository = transportCheckerRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit> Handle(ReScheduleUpdateRequest request, CancellationToken cancellationToken)
        {
            //  var validationResult = await _validator.ValidateAsync(request);
            await  _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheck(request.oldTransportId, request.ScheduleId);
            int? oldScheduleId =  await _ITransportRepository.ReScheduleUpdate(request, cancellationToken);
            if(oldScheduleId.HasValue)
            {
                await _transportScheduleCalculateRepository.CalculateByScheduleId(oldScheduleId.Value, cancellationToken);
            }
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.ScheduleId, cancellationToken);

            return Unit.Value;

        }
    }
}
