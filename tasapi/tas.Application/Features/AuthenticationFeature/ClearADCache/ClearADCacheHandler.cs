using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.CreateBed;
using tas.Application.Repositories;

namespace tas.Application.Features.AuthenticationFeature.ClearADCache
{


    public sealed class ClearADCacheHandler : IRequestHandler<ClearADCacheRequest, Unit>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ClearADCacheHandler(IUnitOfWork unitOfWork,IAuthenticationRepository AuthenticationRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ClearADCacheRequest request, CancellationToken cancellationToken)
        {
             await _AuthenticationRepository.ClearADCache(request, cancellationToken);
            return Unit.Value;
        }
    }
}
