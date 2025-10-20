using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupByType
{

    public sealed class RequestDocumentGroupByTypeHandler : IRequestHandler<RequestDocumentGroupByTypeRequest, RequestDocumentGroupByTypeResponse>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;

        public RequestDocumentGroupByTypeHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<RequestDocumentGroupByTypeResponse> Handle(RequestDocumentGroupByTypeRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupConfigRepository.GetGroupsAndMembersByType(request, cancellationToken);
            return data;
        }
    }
}
