using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelAdd
{

    public sealed class GetRequestDocumentExternalTravelAddHandler : IRequestHandler<GetRequestDocumentExternalTravelAddRequest, GetRequestDocumentExternalTravelAddResponse>
    {
        private readonly IRequestExternalTravelAddRepository _RequestExternalTravelAddRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentExternalTravelAddHandler(IRequestExternalTravelAddRepository RequestExternalTravelAddRepository, IMapper mapper)
        {
            _RequestExternalTravelAddRepository = RequestExternalTravelAddRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentExternalTravelAddResponse> Handle(GetRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestExternalTravelAddRepository.GetRequestDocumentExternalTravelAdd(request, cancellationToken);
            return data;
        }
    }
}
