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

namespace tas.Application.Features.TransportFeature.RemoveExternalTransport
{

    public sealed class RemoveExternalTransportHandler : IRequestHandler<RemoveExternalTransportRequest, Unit>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RemoveExternalTransportRequest> _validator;
        public RemoveExternalTransportHandler(ITransportRepository TransportRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<RemoveExternalTransportRequest> validator)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Unit> Handle(RemoveExternalTransportRequest request, CancellationToken cancellationToken)
        {
          //  var validationResult = await _validator.ValidateAsync(request);

            await _ITransportRepository.RemoveExternalTransport(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
