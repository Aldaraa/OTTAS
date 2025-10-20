using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange
{

    public sealed class GetRequestDocumentProfileChangeHandler : IRequestHandler<GetRequestDocumentProfileChangeRequest, GetRequestDocumentProfileChangeResponse>
    {
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeEmployeeRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentProfileChangeHandler(IRequestDocumentProfileChangeEmployeeRepository RequestDocumentProfileChangeEmployeeRepository, IMapper mapper)
        {
            _RequestDocumentProfileChangeEmployeeRepository = RequestDocumentProfileChangeEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentProfileChangeResponse> Handle(GetRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentProfileChangeEmployeeRepository.GetRequestDocumentProfile(request, cancellationToken);
            return data;

        }
    }
}
