using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove
{

    public sealed class GetRequestDocumentSiteTravelRemoveHandler : IRequestHandler<GetRequestDocumentSiteTravelRemoveRequest, GetRequestDocumentSiteTravelRemoveResponse>
    {
        private readonly IRequestSiteTravelRemoveRepository _RequestSiteTravelRemoveRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentSiteTravelRemoveHandler(IRequestSiteTravelRemoveRepository RequestSiteTravelRemoveRepository, IMapper mapper)
        {
            _RequestSiteTravelRemoveRepository = RequestSiteTravelRemoveRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentSiteTravelRemoveResponse> Handle(GetRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestSiteTravelRemoveRepository.GetRequestDocumentSiteTravelRemove(request, cancellationToken);
            return data;
        }
    }
}
