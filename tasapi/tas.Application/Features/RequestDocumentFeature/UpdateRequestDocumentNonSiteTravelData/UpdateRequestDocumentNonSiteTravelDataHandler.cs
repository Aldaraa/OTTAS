using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData
{ 
    public sealed class UpdateRequestDocumentNonSiteTravelDataHandler : IRequestHandler<UpdateRequestDocumentNonSiteTravelDataRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentNonSiteTravelRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDocumentNonSiteTravelDataHandler(IUnitOfWork unitOfWork, IRequestDocumentNonSiteTravelRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestDocumentNonSiteTravelDataRequest request, CancellationToken cancellationToken)
        {
              await _RequestDocumentRepository.UpdateRequestDocumentNonSiteTravelData(request, cancellationToken);
              await _unitOfWork.Save(cancellationToken);
             return Unit.Value;

        }
    }
}
