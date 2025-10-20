
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthenticationFeature.Login
{

    public sealed record LoginResponse
    {
        public string Token { get; init; }
        public DateTime Expiration { get; init; }
        public string Username { get; init; }
    }




}
