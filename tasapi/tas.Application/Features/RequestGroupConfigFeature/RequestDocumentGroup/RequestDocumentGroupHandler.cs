using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroup
{

    public sealed class RequestDocumentGroupHandler : IRequestHandler<RequestDocumentGroupRequest, List<RequestDocumentGroupResponse>>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;

        public RequestDocumentGroupHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<RequestDocumentGroupResponse>> Handle(RequestDocumentGroupRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupConfigRepository.GetApproval(request, cancellationToken);
            return data;
        }
    }
}
