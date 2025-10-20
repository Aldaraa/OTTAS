using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupById
{

    public sealed class RequestDocumentGroupByIdHandler : IRequestHandler<RequestDocumentGroupByIdRequest, List<RequestDocumentGroupByIdResponse>>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;

        public RequestDocumentGroupByIdHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<RequestDocumentGroupByIdResponse>> Handle(RequestDocumentGroupByIdRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupConfigRepository.GetGroupsAndMembersById(request, cancellationToken);
            return data;
        }
    }
}
