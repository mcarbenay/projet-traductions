using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    /// <summary>
    /// Service for managing users, authentication, and user-related business logic.
    /// </summary>
    public class UserService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public UserService(PolyglotInitiativeDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public async Task<List<User>> GetAllAsync() => await _db.Users.ToListAsync();

        /// <summary>
        /// Gets a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        public async Task<User?> GetByIdAsync(Guid id) => await _db.Users.FindAsync(id);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The created user.</returns>
        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="user">The updated user data.</param>
        public async Task<bool> UpdateAsync(Guid id, User user) { if (id != user.Id) return false; _db.Entry(user).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        public async Task<bool> DeleteAsync(Guid id) { var u = await _db.Users.FindAsync(id); if (u == null) return false; _db.Users.Remove(u); await _db.SaveChangesAsync(); return true; }

        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Simple password hash for demo (replace with secure hash in production)
        /// <summary>
        /// Hashes a password for secure storage.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        public static string HashPassword(string password)
        {
            // For demo: use SHA256 (not recommended for real apps)
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifies a user's password against the stored hash.
        /// </summary>
        /// <param name="user">The user whose password is to be verified.</param>
        /// <param name="password">The password to verify.</param>
        public static bool VerifyPassword(User user, string password)
        {
            return user.PasswordHash == HashPassword(password);
        }
    }
}
