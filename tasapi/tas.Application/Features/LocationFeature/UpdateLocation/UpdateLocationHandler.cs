using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.LocationFeature.UpdateLocation
{
    public class UpdateLocationHandler : IRequestHandler<UpdateLocationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationRepository _LocationRepository;
        private readonly IMapper _mapper;

        public UpdateLocationHandler(IUnitOfWork unitOfWork, ILocationRepository LocationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _LocationRepository = LocationRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateLocationRequest request, CancellationToken cancellationToken)
        {
            var Location = _mapper.Map<Location>(request);
            await _LocationRepository.CheckDuplicateData(Location, c => c.Code, c => c.Description);
            _LocationRepository.Update(Location);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
