using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentListInpersonate
{

    public sealed class GetDocumentListInpersonateHandler : IRequestHandler<GetDocumentListInpersonateRequest, List<GetDocumentListInpersonateResponse>>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetDocumentListInpersonateHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<List<GetDocumentListInpersonateResponse>> Handle(GetDocumentListInpersonateRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.GetDocumentListInpersonate(request, cancellationToken);
            return data;
        }
    }
}
