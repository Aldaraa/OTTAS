using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.CreateBed;
using tas.Application.Repositories;

namespace tas.Application.Features.AuthenticationFeature.LoginUser
{


    public sealed class LoginUserHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;

        public LoginUserHandler(IAuthenticationRepository AuthenticationRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
        }

        public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            return  await _AuthenticationRepository.LoginUser(request, cancellationToken);
        }
    }
}
