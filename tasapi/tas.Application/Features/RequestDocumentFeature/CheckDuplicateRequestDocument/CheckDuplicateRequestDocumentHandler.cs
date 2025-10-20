using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument
{

    public sealed class CheckDuplicateRequestDocumentHandler : IRequestHandler<CheckDuplicateRequestDocumentRequest, List<CheckDuplicateRequestDocumentResponse>>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public CheckDuplicateRequestDocumentHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<List<CheckDuplicateRequestDocumentResponse>> Handle(CheckDuplicateRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.CheckDuplicateRequestDocument(request, cancellationToken);
            return data;
        }
    }
}
