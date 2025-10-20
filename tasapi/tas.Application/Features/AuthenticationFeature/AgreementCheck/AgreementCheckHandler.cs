using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.CreateBed;
using tas.Application.Repositories;

namespace tas.Application.Features.AuthenticationFeature.AgreementCheck
{


    public sealed class AgreementCheckHandler : IRequestHandler<AgreementCheckRequest, Unit>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AgreementCheckHandler(IUnitOfWork unitOfWork,IAuthenticationRepository AuthenticationRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AgreementCheckRequest request, CancellationToken cancellationToken)
        {
             await _AuthenticationRepository.AgreementCheck(request, cancellationToken);
            return Unit.Value;
        }
    }
}
