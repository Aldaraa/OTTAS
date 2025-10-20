using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelGroup
{

    public sealed class GetNonSiteTravelGroupHandler : IRequestHandler<GetNonSiteTravelGroupRequest, List<GetNonSiteTravelGroupResponse>>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetNonSiteTravelGroupHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetNonSiteTravelGroupResponse>> Handle(GetNonSiteTravelGroupRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.GetNonSiteTravelGroup(request, cancellationToken);
            return data;
        }
    }
}
