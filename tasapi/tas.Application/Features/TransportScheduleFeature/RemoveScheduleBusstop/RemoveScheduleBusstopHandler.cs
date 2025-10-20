using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.RemoveScheduleBusstop
{
    public class RemoveScheduleBusstopHandler : IRequestHandler<RemoveScheduleBusstopRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public RemoveScheduleBusstopHandler(IUnitOfWork unitOfWork, ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(RemoveScheduleBusstopRequest request, CancellationToken cancellationToken)
        {
            await _TransportScheduleRepository.RemoveScheduleScheduleBusstop(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
