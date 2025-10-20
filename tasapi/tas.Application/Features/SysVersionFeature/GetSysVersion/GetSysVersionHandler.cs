using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.SysVersionFeature.GetSysVersion
{

    public sealed class GetSysVersionHandler : IRequestHandler<GetSysVersionRequest, GetSysVersionResponse>
    {
        private readonly ISysVersionRepository _SysVersionRepository;
        private readonly IMapper _mapper;

        public GetSysVersionHandler(ISysVersionRepository SysVersionRepository, IMapper mapper)
        {
            _SysVersionRepository = SysVersionRepository;
            _mapper = mapper;
        }

        public async Task<GetSysVersionResponse> Handle(GetSysVersionRequest request, CancellationToken cancellationToken)
        {
            var data = await _SysVersionRepository.GeVersion(request, cancellationToken);
            return data;
        }
    }
}
