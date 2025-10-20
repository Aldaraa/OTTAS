using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled
{

    public sealed class GetDocumentListCancelledHandler : IRequestHandler<GetDocumentListCancelledRequest, GetDocumentListCancelledResponse>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetDocumentListCancelledHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<GetDocumentListCancelledResponse> Handle(GetDocumentListCancelledRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.GetDocumentListCancelled(request, cancellationToken);
            return data;
        }
    }
}
