using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentRoute
{

    public sealed class RequestDocumentRouteHandler : IRequestHandler<RequestDocumentRouteRequest, List<RequestDocumentRouteResponse>>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;

        public RequestDocumentRouteHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<RequestDocumentRouteResponse>> Handle(RequestDocumentRouteRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupConfigRepository.GetRequestDocumentRoute(request, cancellationToken);
            return data;
        }
    }
}
