using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChangeTemp
{ 
    public sealed class UpdateRequestDocumentProfileChangeHandler : IRequestHandler<UpdateRequestDocumentProfileChangeTempRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeEmployeeRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDocumentProfileChangeHandler(IUnitOfWork unitOfWork, IRequestDocumentProfileChangeEmployeeRepository requestDocumentProfileChangeEmployeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentProfileChangeEmployeeRepository = requestDocumentProfileChangeEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
              await _RequestDocumentProfileChangeEmployeeRepository.UpdateRequestDocumentProfileChangeTemp(request, cancellationToken);
              await _unitOfWork.Save(cancellationToken);
              return Unit.Value;
        }
    }
}
