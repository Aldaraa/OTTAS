using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelRemove
{

    public sealed class GetRequestDocumentExternalTravelRemoveHandler : IRequestHandler<GetRequestDocumentExternalTravelRemoveRequest, GetRequestDocumentExternalTravelRemoveResponse>
    {
        private readonly IRequestExternalTravelRemoveRepository _RequestExternalTravelRemoveRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentExternalTravelRemoveHandler(IRequestExternalTravelRemoveRepository RequestExternalTravelRemoveRepository, IMapper mapper)
        {
            _RequestExternalTravelRemoveRepository = RequestExternalTravelRemoveRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentExternalTravelRemoveResponse> Handle(GetRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestExternalTravelRemoveRepository.GetRequestDocumentExternalTravelRemove(request, cancellationToken);
            return data;
        }
    }
}
