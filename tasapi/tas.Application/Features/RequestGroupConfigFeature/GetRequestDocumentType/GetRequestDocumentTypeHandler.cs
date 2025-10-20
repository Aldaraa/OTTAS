using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestGroupConfigFeature.GetRequestDocumentType
{

    public sealed class GetRequestDocumentTypeHandler : IRequestHandler<GetRequestDocumentTypeRequest, List<GetRequestDocumentTypeResponse>>
    {
        private readonly IRequestGroupConfigRepository _RequestGroupConfigRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentTypeHandler(IRequestGroupConfigRepository RequestGroupConfigRepository, IMapper mapper)
        {
            _RequestGroupConfigRepository = RequestGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRequestDocumentTypeResponse>> Handle(GetRequestDocumentTypeRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestGroupConfigRepository.GetAllDocuments(request, cancellationToken);
            return data;
        }
    }
}
