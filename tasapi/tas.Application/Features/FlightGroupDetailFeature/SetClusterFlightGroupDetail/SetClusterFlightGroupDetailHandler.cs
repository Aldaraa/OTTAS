using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail
{ 
    public sealed class SetClusterFlightGroupDetailHandler : IRequestHandler<SetClusterFlightGroupDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlightGroupDetailRepository _FlightGroupDetailRepository;
        private readonly IMapper _mapper;

        public SetClusterFlightGroupDetailHandler(IUnitOfWork unitOfWork, IFlightGroupDetailRepository FlightGroupDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _FlightGroupDetailRepository = FlightGroupDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(SetClusterFlightGroupDetailRequest request, CancellationToken cancellationToken)
        {
            //var FlightGroupDetail = _mapper.Map<FlightGroupDetail>(request);
            await  _FlightGroupDetailRepository.SetCluster(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
