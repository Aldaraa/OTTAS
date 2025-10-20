using Application.Features.AuthenticationFeature.Login;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<IntegrationAPIUser> GetUserByUsernameAsync(string username);
        Task<IntegrationAPIUser> GetUserByIdAsync(int userId);
        Task<string> GenerateTokenAsync(string username);
        Task<bool> UpdateLastRequestDateAsync(int userId, DateTime lastRequestDate);


        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    }
}
