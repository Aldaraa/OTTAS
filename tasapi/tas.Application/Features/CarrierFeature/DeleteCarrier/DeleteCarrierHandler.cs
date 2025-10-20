using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CarrierFeature.DeleteCarrier
{

    public sealed class DeleteCarrierHandler : IRequestHandler<DeleteCarrierRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICarrierRepository _CarrierRepository;
        private readonly IMapper _mapper;

        public DeleteCarrierHandler(IUnitOfWork unitOfWork, ICarrierRepository CarrierRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CarrierRepository = CarrierRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteCarrierRequest request, CancellationToken cancellationToken)
        {
            var Carrier = _mapper.Map<Carrier>(request);
            _CarrierRepository.Delete(Carrier);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
