using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UseTheOps.PolyglotInitiative.Data;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Services
{
    public class UserService
    {
        private readonly PolyglotInitiativeDbContext _db;
        public UserService(PolyglotInitiativeDbContext db) { _db = db; }

        public async Task<List<User>> GetAllAsync() => await _db.Users.ToListAsync();
        public async Task<User?> GetByIdAsync(Guid id) => await _db.Users.FindAsync(id);
        public async Task<User> CreateAsync(User user) { _db.Users.Add(user); await _db.SaveChangesAsync(); return user; }
        public async Task<bool> UpdateAsync(Guid id, User user) { if (id != user.Id) return false; _db.Entry(user).State = EntityState.Modified; await _db.SaveChangesAsync(); return true; }
        public async Task<bool> DeleteAsync(Guid id) { var u = await _db.Users.FindAsync(id); if (u == null) return false; _db.Users.Remove(u); await _db.SaveChangesAsync(); return true; }
    }
}
