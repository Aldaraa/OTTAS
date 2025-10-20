using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportModeFeature.GetAllTransportMode
{

    public sealed class GetAllTransportModeHandler : IRequestHandler<GetAllTransportModeRequest, List<GetAllTransportModeResponse>>
    {
        private readonly ITransportModeRepository _TransportModeRepository;
        private readonly IMapper _mapper;

        public GetAllTransportModeHandler(ITransportModeRepository TransportModeRepository, IMapper mapper)
        {
            _TransportModeRepository = TransportModeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllTransportModeResponse>> Handle(GetAllTransportModeRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var TransportModes = await _TransportModeRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllTransportModeResponse>>(TransportModes);
            }
            else {
                var users = await _TransportModeRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllTransportModeResponse>>(users);
            }

        }
    }
}
