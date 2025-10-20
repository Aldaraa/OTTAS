using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.UpdateDescription
{
    public class UpdateDescriptionHandler : IRequestHandler<UpdateDescriptionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public UpdateDescriptionHandler(IUnitOfWork unitOfWork, ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateDescriptionRequest request, CancellationToken cancellationToken)
        {
            await _TransportScheduleRepository.UpdateScheduleDescription(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
