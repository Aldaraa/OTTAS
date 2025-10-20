using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.DeleteFlightGroupMaster
{

    public sealed class DeleteFlightGroupMasterHandler : IRequestHandler<DeleteFlightGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlightGroupMasterRepository _FlightGroupMasterRepository;
        private readonly IMapper _mapper;

        public DeleteFlightGroupMasterHandler(IUnitOfWork unitOfWork, IFlightGroupMasterRepository FlightGroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _FlightGroupMasterRepository = FlightGroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteFlightGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var FlightGroupMaster = _mapper.Map<FlightGroupMaster>(request);
            _FlightGroupMasterRepository.Delete(FlightGroupMaster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
