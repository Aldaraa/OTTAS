using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChangeTemp
{

    public sealed class GetRequestDocumentProfileChangeTempHandler : IRequestHandler<GetRequestDocumentProfileChangeTempRequest, GetRequestDocumentProfileChangeTempResponse>
    {
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeTempEmployeeRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentProfileChangeTempHandler(IRequestDocumentProfileChangeEmployeeRepository RequestDocumentProfileChangeEmployeeRepository, IMapper mapper)
        {
            _RequestDocumentProfileChangeTempEmployeeRepository = RequestDocumentProfileChangeEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentProfileChangeTempResponse> Handle(GetRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            var data = await _RequestDocumentProfileChangeTempEmployeeRepository.GetRequestDocumentProfileTemp(request, cancellationToken);
            return data;

        }
    }
}
