using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel
{ 
    public sealed class GetRequestDocumentNonSiteTravelHandler : IRequestHandler<GetRequestDocumentNonSiteTravelRequest, GetRequestDocumentNonSiteTravelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentNonSiteTravelRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentNonSiteTravelHandler(IUnitOfWork unitOfWork, IRequestDocumentNonSiteTravelRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentNonSiteTravelResponse> Handle(GetRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
            return await _RequestDocumentRepository.GetRequestDocumentNonSiteTravel(request, cancellationToken);

              

        }
    }
}
