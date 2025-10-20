using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.CreateFlightGroupMaster
{
    public sealed class SetClusterFlightGroupDetailHandler : IRequestHandler<CreateFlightGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlightGroupMasterRepository _FlightGroupMasterRepository;
        private readonly IMapper _mapper;

        public SetClusterFlightGroupDetailHandler(IUnitOfWork unitOfWork, IFlightGroupMasterRepository FlightGroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _FlightGroupMasterRepository = FlightGroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateFlightGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var FlightGroupMaster = _mapper.Map<FlightGroupMaster>(request);
            _FlightGroupMasterRepository.Create(FlightGroupMaster);
            await _unitOfWork.Save(cancellationToken);
            await _FlightGroupMasterRepository.CreateShiftConfig(FlightGroupMaster.Id, request.DayPattern);
            return Unit.Value;
        }
    }
}
