using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GenerateCompletedDeclinedChange
{ 
    public sealed class GenerateCompletedDeclinedChangeHandler : IRequestHandler<GenerateCompletedDeclinedChangeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _requestDocumentRepository;

        public GenerateCompletedDeclinedChangeHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _requestDocumentRepository = requestDocumentRepository;
        }

        // TODO: Only available for developer use.
        public async Task<Unit> Handle(GenerateCompletedDeclinedChangeRequest request, CancellationToken cancellationToken)
        {
            // TODO: Only available for developer use.
            await _requestDocumentRepository.GenerateCompletedDeclinedChange(request, cancellationToken);

            return Unit.Value;
        }
    }
}
