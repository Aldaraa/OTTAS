
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.GetSessionList
{

    public sealed class GetSessionListHandler : IRequestHandler<GetSessionListRequest, List<GetSessionListResponse>>
    {
        private readonly IJobExecuteServiceRepository _JobExecuteServiceRepository;
        private readonly IMapper _mapper;

        public GetSessionListHandler(IJobExecuteServiceRepository JobExecuteServiceRepository, IMapper mapper)
        {
            _JobExecuteServiceRepository = JobExecuteServiceRepository;
            _mapper = mapper;
        }

        public async Task<List<GetSessionListResponse>> Handle(GetSessionListRequest request, CancellationToken cancellationToken)
        {

            return await _JobExecuteServiceRepository.GetCurrentSessions(request, cancellationToken);


        }
    }
}
