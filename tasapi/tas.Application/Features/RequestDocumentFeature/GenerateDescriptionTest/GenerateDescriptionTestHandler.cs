using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GenerateDescriptionTest
{ 
    public sealed class GenerateDescriptionTestHandler : IRequestHandler<GenerateDescriptionTestRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _requestDocumentRepository;

        public GenerateDescriptionTestHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _requestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(GenerateDescriptionTestRequest request, CancellationToken cancellationToken)
        {

             await _requestDocumentRepository.GenerateDescriptionTest(request, cancellationToken);

            return Unit.Value;
        }
    }
}
