using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CarrierFeature.CreateCarrier
{
    public sealed class CreateCarrierHandler : IRequestHandler<CreateCarrierRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICarrierRepository _CarrierRepository;
        private readonly IMapper _mapper;

        public CreateCarrierHandler(IUnitOfWork unitOfWork, ICarrierRepository CarrierRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CarrierRepository = CarrierRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateCarrierRequest request, CancellationToken cancellationToken)
        {
            var Carrier = _mapper.Map<Carrier>(request);
            await _CarrierRepository.CheckDuplicateData(Carrier, c => c.Code, c => c.Description);
            _CarrierRepository.Create(Carrier);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
