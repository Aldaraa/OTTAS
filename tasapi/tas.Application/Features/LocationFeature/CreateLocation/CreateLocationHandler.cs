using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.LocationFeature.CreateLocation
{
    public sealed class CreateLocationHandler : IRequestHandler<CreateLocationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationRepository _LocationRepository;
        private readonly IMapper _mapper;

        public CreateLocationHandler(IUnitOfWork unitOfWork, ILocationRepository LocationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _LocationRepository = LocationRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateLocationRequest request, CancellationToken cancellationToken)
        {
            var Location = _mapper.Map<Location>(request);
            await _LocationRepository.CheckDuplicateData(Location, c => c.Code, c=> c.Description);
            _LocationRepository.Create(Location);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
