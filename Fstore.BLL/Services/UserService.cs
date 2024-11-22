using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net; // Make sure to install BCrypt.Net package for hashing
using Fstore.DAL.Models;
using Fstore.DAL.Repositories;

namespace Fstore.BLL.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Add a new user with password hashing
        public async Task<User> AddUserAsync(string username, string email, string password)
        {
            // Check if the email already exists
            if (await CheckIfEmailExistsAsync(email))
            {
                throw new ArgumentException("Email already exists.");
            }

            // Hash the password
            string passwordHash = HashPassword(password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = "User", // Default role logic
                CreatedAt = DateTime.UtcNow
            };

            return await _userRepository.AddUserAsync(user);
        }

        // Hash password using BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Check if the email already exists
        public async Task<bool> CheckIfEmailExistsAsync(string email)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            return existingUser != null;
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        // Update user details
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }

            return await _userRepository.UpdateUserAsync(user) != null;
        }

        // Delete user
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        // Authenticate user
        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user; // Return user if authenticated
            }
            return null; // Return null if authentication fails
        }

        // Verify password
        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash); // Compare password with stored hash
        }

        // Reset user password
        public async Task<bool> ResetUserPasswordAsync(string email, string newPassword)
        {
            // Retrieve the user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                // Hash the new password before saving
                user.PasswordHash = HashPassword(newPassword);

                // Update the user in the repository
                return await _userRepository.UpdateUserAsync(user) != null;
            }
            return false; // User not found
        }
    }
}
