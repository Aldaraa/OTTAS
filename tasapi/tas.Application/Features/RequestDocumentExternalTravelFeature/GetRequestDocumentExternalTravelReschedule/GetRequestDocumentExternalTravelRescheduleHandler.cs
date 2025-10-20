using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelReschedule
{

    public sealed class GetRequestDocumentExternalTravelRescheduleHandler : IRequestHandler<GetRequestDocumentExternalTravelRescheduleRequest, GetRequestDocumentExternalTravelRescheduleResponse>
    {
        private readonly IRequestExternalTravelReScheduleRepository _RequestExternalTravelRescheduleRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentExternalTravelRescheduleHandler(IRequestExternalTravelReScheduleRepository RequestExternalTravelRescheduleRepository, IMapper mapper)
        {
            _RequestExternalTravelRescheduleRepository = RequestExternalTravelRescheduleRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentExternalTravelRescheduleResponse> Handle(GetRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestExternalTravelRescheduleRepository.GetRequestDocumentExternalTravelReschedule(request, cancellationToken);
            return data;
        }
    }
}
