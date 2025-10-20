using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.ExistingBookingRequestDocument
{

    public sealed class ExistingBookingRequestDocumentHandler : IRequestHandler<ExistingBookingRequestDocumentRequest, ExistingBookingRequestDocumentResponse>
    {
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public ExistingBookingRequestDocumentHandler(IRequestDocumentRepository RequestDocumentRepository, IMapper mapper)
        {
            _RequestDocumentRepository = RequestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<ExistingBookingRequestDocumentResponse> Handle(ExistingBookingRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentRepository.ExistingBookingRequestDocument(request, cancellationToken);
            return data;
        }
    }
}
