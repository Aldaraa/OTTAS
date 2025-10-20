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

namespace tas.Application.Features.TransportFeature.ReScheduleExternalTransport
{

    public sealed class ReScheduleExternalTransportHandler : IRequestHandler<ReScheduleExternalTransportRequest, Unit>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ReScheduleExternalTransportRequest> _validator;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        public ReScheduleExternalTransportHandler(ITransportRepository TransportRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ReScheduleExternalTransportRequest> validator, ITransportCheckerRepository transportCheckerRepository)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _transportCheckerRepository = transportCheckerRepository;
        }

        public async Task<Unit> Handle(ReScheduleExternalTransportRequest request, CancellationToken cancellationToken)
        {
            //  var validationResult = await _validator.ValidateAsync(request);
            await  _transportCheckerRepository.TransportExternalRescheduleValidCheck(request.oldTransportId, request.ScheduleId);
            await _ITransportRepository.ReScheduleExternalTransport(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
