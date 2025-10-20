using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.SysVersionHistoryFeature.GetSysVersionHistory
{

    public sealed class GetSysVersionHistoryHandler : IRequestHandler<GetSysVersionHistoryRequest, List<GetSysVersionHistoryResponse>>
    {
        private readonly ISysVersionRepository _SysVersionHistoryRepository;
        private readonly IMapper _mapper;

        public GetSysVersionHistoryHandler(ISysVersionRepository SysVersionRepository, IMapper mapper)
        {
            _SysVersionHistoryRepository = SysVersionRepository;
            _mapper = mapper;
        }

        public async Task<List<GetSysVersionHistoryResponse>> Handle(GetSysVersionHistoryRequest request, CancellationToken cancellationToken)
        {
            var data = await _SysVersionHistoryRepository.GeVersionHistory(request, cancellationToken);
            return data;
        }
    }
}
