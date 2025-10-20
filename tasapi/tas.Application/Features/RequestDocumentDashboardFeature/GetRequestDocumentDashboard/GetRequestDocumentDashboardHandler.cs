using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetAllEmployee;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentDashboardFeature.GetRequestDocumentDashboard
{

    public sealed class GetRequestDocumentDashboardHandler : IRequestHandler<GetRequestDocumentDashboardRequest, GetRequestDocumentDashboardResponse>
    {
        private readonly IRequestDeMobilisationRepository _RequestDeMobilisationRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentDashboardHandler(IRequestDeMobilisationRepository RequestDeMobilisationRepository, IMapper mapper)
        {
            _RequestDeMobilisationRepository = RequestDeMobilisationRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentDashboardResponse> Handle(GetRequestDocumentDashboardRequest request, CancellationToken cancellationToken)
        {
            //var data = await _RequestDeMobilisationRepository.GetRequestDocumentDashboard(request, cancellationToken);
            //return data;

            return new GetRequestDocumentDashboardResponse();

        }
    }
}
