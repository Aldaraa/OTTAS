using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule
{

    public sealed class GetRequestDocumentSiteTravelRescheduleHandler : IRequestHandler<GetRequestDocumentSiteTravelRescheduleRequest, GetRequestDocumentSiteTravelRescheduleResponse>
    {
        private readonly IRequestSiteTravelRescheduleRepository _RequestSiteTravelRescheduleRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentSiteTravelRescheduleHandler(IRequestSiteTravelRescheduleRepository RequestSiteTravelRescheduleRepository, IMapper mapper)
        {
            _RequestSiteTravelRescheduleRepository = RequestSiteTravelRescheduleRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentSiteTravelRescheduleResponse> Handle(GetRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestSiteTravelRescheduleRepository.GetRequestDocumentSiteTravelReschedule(request, cancellationToken);
            return data;
        }
    }
}
