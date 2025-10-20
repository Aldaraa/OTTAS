using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupFeature.GetAllRequestGroup
{

    public sealed class GetAllRequestGroupHandler : IRequestHandler<GetAllRequestGroupRequest, List<GetAllRequestGroupResponse>>
    {
        private readonly IRequestGroupRepository _RequestGroupRepository;
        private readonly IMapper _mapper;

        public GetAllRequestGroupHandler(IRequestGroupRepository RequestGroupRepository, IMapper mapper)
        {
            _RequestGroupRepository = RequestGroupRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestGroupResponse>> Handle(GetAllRequestGroupRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupRepository.GetAllData(request, cancellationToken);
            return data;
        }
    }
}
