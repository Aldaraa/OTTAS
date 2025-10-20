using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.CarrierFeature.GetAllCarrier
{

    public sealed class GetAllCarrierHandler : IRequestHandler<GetAllCarrierRequest, List<GetAllCarrierResponse>>
    {
        private readonly ICarrierRepository _CarrierRepository;
        private readonly IMapper _mapper;

        public GetAllCarrierHandler(ICarrierRepository CarrierRepository, IMapper mapper)
        {
            _CarrierRepository = CarrierRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllCarrierResponse>> Handle(GetAllCarrierRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var Carriers = await _CarrierRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllCarrierResponse>>(Carriers);
            }
            else {
                var users = await _CarrierRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllCarrierResponse>>(users);
            }

        }
    }
}
