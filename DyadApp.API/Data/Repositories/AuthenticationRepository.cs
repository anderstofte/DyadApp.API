﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DyadApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DyadApp.API.Data.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DyadAppContext _context;

        public AuthenticationRepository(DyadAppContext context)
        {
            _context = context;
        }

        public async Task<Signup> GetSignupByToken(string token)
        {
            return await _context.Signups
                .Where(s => s.Token == token && s.ExpirationDate > DateTime.Now && s.AcceptDate == null)
                .SingleOrDefaultAsync();
        }

        public async Task<User> AuthenticateUser(string email, string password)
        {
            return await _context.Users.Select(u => new User
            {
                UserId = u.UserId,
                Email = u.Email,
                Password = u.Password,
                Salt = u.Salt,
                Verified = u.Verified,
                RefreshTokens = u.RefreshTokens
            }).Where(x => x.Email == email && x.Verified).SingleOrDefaultAsync();
        }

        public async Task<User> GetUserCredentialsByEmail(string email)
        {
            return await _context.Users.Select(u => new User
            {
                UserId = u.UserId,
                Email = u.Email,
                Password = u.Password,
                Salt = u.Salt,
                Verified = u.Verified,
                RefreshTokens = u.RefreshTokens
            }).Where(x => x.Email == email).SingleOrDefaultAsync();
        }

        public async Task UpdatePassword(UserPassword model)
        {
            _context.Update(model);
            await SaveChangesAsync();
        }

        public async Task CreateTokenAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTokenAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<RefreshToken> GetRefreshToken(int userId, string token)
        {
            return _context.RefreshTokens
                .Where(x => x.UserId == userId && x.Token == token)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetResetPasswordToken(string token, string email)
        {
            return await _context.Users
                .Where(x => x.Email == email)
                .Select(x => new User
                {
                    UserId = x.UserId,
                    ResetPasswordTokens = x.ResetPasswordTokens.Select(rpt => new ResetPasswordToken
                    {
                        Token = rpt.Token
                    }).Where(rpt => rpt.Token == token).ToList(),
                    Password = x.Password,
                    Salt = x.Salt
                }).SingleOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}