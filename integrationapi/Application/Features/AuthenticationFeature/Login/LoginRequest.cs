using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationFeature.Login
{
    public sealed record LoginRequest(string Username, string Password) :  IRequest<LoginResponse>;

}
