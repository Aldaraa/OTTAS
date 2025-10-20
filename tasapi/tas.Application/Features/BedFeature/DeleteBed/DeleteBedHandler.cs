using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.DeleteBed
{

    public sealed class DeleteBedHandler : IRequestHandler<DeleteBedRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBedRepository _BedRepository;
        private readonly IMapper _mapper;

        public DeleteBedHandler(IUnitOfWork unitOfWork, IBedRepository BedRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _BedRepository = BedRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteBedRequest request, CancellationToken cancellationToken)
        {
            var Bed = _mapper.Map<Bed>(request);
            _BedRepository.Delete(Bed);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
