using Application.Common.Exceptions;
using Application.Features.AuthenticationFeature.Login;
using Application.Repositories;
using Application.Service;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class AuthenticationRepository :  IAuthenticationRepository
    {
        protected readonly DataContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;
        public AuthenticationRepository(DataContext context, PasswordService passwordService, TokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<IntegrationAPIUser?> GetUserByUsernameAsync(string username)
        {
            return await _context.IntegrationAPIUser.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IntegrationAPIUser?> GetUserByIdAsync(int userId)
        {
            return await _context.IntegrationAPIUser.FindAsync(userId);
        }



        public async Task<string> GenerateTokenAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new BadRequestException("User not found");
            }

            var token = _tokenService.GenerateToken(username);
            user.CurrentToken = token;
            user.TokenExpirationDate = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> UpdateLastRequestDateAsync(int userId, DateTime lastRequestDate)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.LastRequestDate = lastRequestDate;
            user.DateUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await GetUserByUsernameAsync(request.Username);

      var otinfouserpass =      _passwordService.HashPassword("OTInfoTas$pa$$889#");
            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            var token = await GenerateTokenAsync(request.Username);
            await UpdateLastRequestDateAsync(user.Id, DateTime.UtcNow);

            return new LoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(6),
                Username = request.Username,
            };
        }

    }
}
