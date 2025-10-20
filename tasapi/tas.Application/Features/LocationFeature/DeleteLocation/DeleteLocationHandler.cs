using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.LocationFeature.DeleteLocation
{

    public sealed class DeleteLocationHandler : IRequestHandler<DeleteLocationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationRepository _LocationRepository;
        private readonly IMapper _mapper;

        public DeleteLocationHandler(IUnitOfWork unitOfWork, ILocationRepository LocationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _LocationRepository = LocationRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteLocationRequest request, CancellationToken cancellationToken)
        {
            var Location = _mapper.Map<Location>(request);
            _LocationRepository.Delete(Location);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
