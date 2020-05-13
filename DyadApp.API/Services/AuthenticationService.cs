﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DyadApp.API.Data.Repositories;
using DyadApp.API.Extensions;
using DyadApp.API.Helpers;
using DyadApp.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace DyadApp.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ISecretKeyService _keyService;

        public AuthenticationService(ISecretKeyService keyService, IAuthenticationRepository authenticationRepository)
        {
            _keyService = keyService;
            _authenticationRepository = authenticationRepository;
        }

        public async Task<AuthenticationTokens> Authenticate(string email, string password)
        {
            var user = await _authenticationRepository.GetUserCredentialsByEmail(email);
            if (user == null)
            {
                return null;
            }

            var isSubmittedPasswordValid = user.ValidatePassword(password);
            if (!isSubmittedPasswordValid)
            {
                return null;
            }

            return await GenerateTokens(user.UserId);
        }

        public async Task<AuthenticationTokens> GenerateTokens(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.CreateToken(SetUpToken(userId));

            var refreshToken = RefreshTokenHelper.Generate(userId);

            await _authenticationRepository.CreateTokenAsync(refreshToken);

            var authenticationTokens = new AuthenticationTokens
            {
                AccessToken = tokenHandler.WriteToken(accessToken),
                RefreshToken = refreshToken.Token
            };

            return authenticationTokens;
        }

        private SecurityTokenDescriptor SetUpToken(int userId)
        {
            var key = Encoding.ASCII.GetBytes(_keyService.GetSecretKey());
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
        }
    }
}
