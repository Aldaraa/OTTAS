using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETD
{
    public class UpdateTransportScheduleRealETDHandler : IRequestHandler<UpdateTransportScheduleRealETDRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public UpdateTransportScheduleRealETDHandler(IUnitOfWork unitOfWork, ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateTransportScheduleRealETDRequest request, CancellationToken cancellationToken)
        {
            await _TransportScheduleRepository.UpdateTransportScheduleRealETD(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
