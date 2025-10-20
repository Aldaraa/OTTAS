using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupEmpLines
{

    public sealed class RequestDocumentGroupEmpLinesHandler : IRequestHandler<RequestDocumentGroupEmpLinesRequest, List<RequestDocumentGroupEmpLinesResponse>>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;

        public RequestDocumentGroupEmpLinesHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<RequestDocumentGroupEmpLinesResponse>> Handle(RequestDocumentGroupEmpLinesRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupConfigRepository.GetEmployeeLineManagers(request, cancellationToken);
            return data;
        }
    }
}
