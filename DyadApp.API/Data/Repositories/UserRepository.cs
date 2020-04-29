﻿using System.Linq;
using System.Threading.Tasks;
using DyadApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DyadApp.API.Data.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly DyadAppContext _context;

        public UserRepository(DyadAppContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesUserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.Where(u => u.UserId == id).SingleOrDefaultAsync();
        }

        public async Task<UserPassword> GetUserPasswordByUserId(int id)
        {
            return await _context.UserPasswords.Where(x => x.UserId == id).SingleOrDefaultAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.Where(x => x.Email == email).SingleOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}