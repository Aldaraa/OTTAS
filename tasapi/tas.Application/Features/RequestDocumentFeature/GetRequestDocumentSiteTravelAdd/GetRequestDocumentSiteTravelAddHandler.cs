using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd
{

    public sealed class GetRequestDocumentSiteTravelAddHandler : IRequestHandler<GetRequestDocumentSiteTravelAddRequest, GetRequestDocumentSiteTravelAddResponse>
    {
        private readonly IRequestSiteTravelAddRepository _RequestSiteTravelAddRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentSiteTravelAddHandler(IRequestSiteTravelAddRepository RequestSiteTravelAddRepository, IMapper mapper)
        {
            _RequestSiteTravelAddRepository = RequestSiteTravelAddRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentSiteTravelAddResponse> Handle(GetRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestSiteTravelAddRepository.GetRequestDocumentSiteTravelAdd(request, cancellationToken);
            return data;
        }
    }
}
