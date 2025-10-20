using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.FlightGroupMasterFeature.UpdateFlightGroupMaster
{
    public class UpdateFlightGroupMasterHandler : IRequestHandler<UpdateFlightGroupMasterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFlightGroupMasterRepository _FlightGroupMasterRepository;
        private readonly IMapper _mapper;

        public UpdateFlightGroupMasterHandler(IUnitOfWork unitOfWork, IFlightGroupMasterRepository FlightGroupMasterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _FlightGroupMasterRepository = FlightGroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateFlightGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var FlightGroupMaster = _mapper.Map<FlightGroupMaster>(request);
            _FlightGroupMasterRepository.Update(FlightGroupMaster);
            await _unitOfWork.Save(cancellationToken);
          //  await _FlightGroupMasterRepository.CreateShiftConfig(FlightGroupMaster.Id, request.da);
            return Unit.Value;
        }
    }
}
