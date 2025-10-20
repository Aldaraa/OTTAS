using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.GetRequestDocumentDeMobilisation
{

    public sealed class GetRequestDocumentDeMobilisationHandler : IRequestHandler<GetRequestDocumentDeMobilisationRequest, GetRequestDocumentDeMobilisationResponse>
    {
        private readonly IRequestDeMobilisationRepository _RequestDeMobilisationRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentDeMobilisationHandler(IRequestDeMobilisationRepository RequestDeMobilisationRepository, IMapper mapper)
        {
            _RequestDeMobilisationRepository = RequestDeMobilisationRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentDeMobilisationResponse> Handle(GetRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDeMobilisationRepository.GetRequestDocumentDeMobilisation(request, cancellationToken);
            return data;

            return new GetRequestDocumentDeMobilisationResponse();

        }
    }
}
