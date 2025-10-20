using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestAirportFeature.UpdateRequestAirport
{
    public class UpdateClusterHandler : IRequestHandler<UpdateRequestAirportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestAirportRepository _RequestAirportRepository;
        private readonly IMapper _mapper;

        public UpdateClusterHandler(IUnitOfWork unitOfWork, IRequestAirportRepository RequestAirportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestAirportRepository = RequestAirportRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestAirportRequest request, CancellationToken cancellationToken)
        {
            await _RequestAirportRepository.UpdateAirport(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
