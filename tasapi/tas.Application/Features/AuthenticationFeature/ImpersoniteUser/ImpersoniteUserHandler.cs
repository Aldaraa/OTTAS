using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.CreateBed;
using tas.Application.Repositories;

namespace tas.Application.Features.AuthenticationFeature.ImpersoniteUser
{


    public sealed class ImpersoniteUserHandler : IRequestHandler<ImpersoniteUserRequest, ImpersoniteUserResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;

        public ImpersoniteUserHandler(IAuthenticationRepository AuthenticationRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
        }

        public async Task<ImpersoniteUserResponse> Handle(ImpersoniteUserRequest request, CancellationToken cancellationToken)
        {
            return  await _AuthenticationRepository.ImpersoniteUserData(request, cancellationToken);
        }
    }
}
