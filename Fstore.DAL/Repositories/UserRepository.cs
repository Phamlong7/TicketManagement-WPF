using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fstore.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Fstore.DAL.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context; // Assuming you have a DbContext called AppDbContext

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Create a new user
        public async Task<bool> CheckIfEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Tickets) // Include tickets if needed
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Tickets) // Include tickets if needed
                .ToListAsync();
        }

        // Update user details
        public async Task<User?> UpdateUserAsync(User user)
        {
            var existingUser = await GetUserByIdAsync(user.Id);
            if (existingUser == null) return null;

            existingUser.Username = user.Username;
            existingUser.Role = user.Role;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        // Delete user
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
