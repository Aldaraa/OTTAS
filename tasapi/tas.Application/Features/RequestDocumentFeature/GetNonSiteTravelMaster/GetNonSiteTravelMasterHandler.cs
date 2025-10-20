using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelMaster
{

    public sealed class GetNonSiteTravelMasterHandler : IRequestHandler<GetNonSiteTravelMasterRequest, GetNonSiteTravelMasterResponse>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetNonSiteTravelMasterHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<GetNonSiteTravelMasterResponse> Handle(GetNonSiteTravelMasterRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.GetMaster(request, cancellationToken);
            return data;
        }
    }
}
