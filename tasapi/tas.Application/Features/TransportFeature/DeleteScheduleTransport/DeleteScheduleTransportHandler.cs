using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.DeleteScheduleTransport
{

    public sealed class DeleteScheduleTransportHandler : IRequestHandler<DeleteScheduleTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;

        public DeleteScheduleTransportHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteScheduleTransportRequest request, CancellationToken cancellationToken)
        {
            await  _TransportRepository.DeleteSchedules(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }

}
