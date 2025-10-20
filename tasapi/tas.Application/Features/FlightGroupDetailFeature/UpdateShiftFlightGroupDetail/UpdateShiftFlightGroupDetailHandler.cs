using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupDetailFeature.UpdateShiftFlightGroupDetail
{ 
    public sealed class UpdateShiftFlightGroupDetailHandler : IRequestHandler<UpdateShiftFlightGroupDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlightGroupDetailRepository _FlightGroupDetailRepository;
        private readonly IMapper _mapper;

        public UpdateShiftFlightGroupDetailHandler(IUnitOfWork unitOfWork, IFlightGroupDetailRepository FlightGroupDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _FlightGroupDetailRepository = FlightGroupDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(UpdateShiftFlightGroupDetailRequest request, CancellationToken cancellationToken)
        {
            await  _FlightGroupDetailRepository.UpdateShift(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
