using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentList
{

    public sealed class GetDocumentListHandler : IRequestHandler<GetDocumentListRequest, GetDocumentListResponse>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetDocumentListHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<GetDocumentListResponse> Handle(GetDocumentListRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.GetDocumentList(request, cancellationToken);
            return data;
        }
    }
}
