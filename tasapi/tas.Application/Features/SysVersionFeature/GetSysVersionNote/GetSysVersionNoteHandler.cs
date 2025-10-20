using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote
{

    public sealed class GetSysVersionNoteHandler : IRequestHandler<GetSysVersionNoteRequest, GetSysVersionNoteResponse>
    {
        private readonly ISysVersionRepository _SysVersionNoteRepository;
        private readonly IMapper _mapper;

        public GetSysVersionNoteHandler(ISysVersionRepository SysVersionRepository, IMapper mapper)
        {
            _SysVersionNoteRepository = SysVersionRepository;
            _mapper = mapper;
        }

        public async Task<GetSysVersionNoteResponse> Handle(GetSysVersionNoteRequest request, CancellationToken cancellationToken)
        {
            var data = await _SysVersionNoteRepository.GeVersionNote(request, cancellationToken);
            return data;
        }
    }
}
