using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.DashboardRequestFeature.GetDocumentDashboard
{

    public sealed class GetDocumentDashboardHandler : IRequestHandler<GetDocumentDashboardRequest, GetDocumentDashboardResponse>
    {
        private readonly IDashboardRequestRepository _DashboardRequestRepository;
        private readonly IMapper _mapper;

        public GetDocumentDashboardHandler(IDashboardRequestRepository DashboardRequestRepository, IMapper mapper)
        {
            _DashboardRequestRepository = DashboardRequestRepository;
            _mapper = mapper;
        }

        public async Task<GetDocumentDashboardResponse> Handle(GetDocumentDashboardRequest request, CancellationToken cancellationToken)
        {
            var data = await _DashboardRequestRepository.GetDocumentDashboard(request, cancellationToken);
            return data;
        }
    }
}
