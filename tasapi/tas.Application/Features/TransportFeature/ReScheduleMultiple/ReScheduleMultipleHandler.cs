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

namespace tas.Application.Features.TransportFeature.ReScheduleMultiple
{

    public sealed class ReScheduleMultipleHandler : IRequestHandler<ReScheduleMultipleRequest, List<ReScheduleMultipleResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ReScheduleMultipleRequest> _validator;
        public ReScheduleMultipleHandler(ITransportRepository TransportRepository, IMapper mapper, IUnitOfWork unitOfWork, IValidator<ReScheduleMultipleRequest> validator)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<List<ReScheduleMultipleResponse>> Handle(ReScheduleMultipleRequest request, CancellationToken cancellationToken)
        {
          //  var validationResult = await _validator.ValidateAsync(request);
                
           var returnData = await _ITransportRepository.ReScheduleMultiple(request, cancellationToken);
            return returnData;

        }
    }
}
