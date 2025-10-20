using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationFeature.Login
{

    public sealed class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IMapper _mapper;

        public LoginHandler(IAuthenticationRepository authenticationRepository, IMapper mapper)
        {
            _authenticationRepository = authenticationRepository;
            _mapper = mapper;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var data = await _authenticationRepository.LoginAsync(request, cancellationToken);
            return data;

        }
    }
}
