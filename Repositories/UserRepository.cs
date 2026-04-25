namespace wisheo_backend_v2.Repositories;

using wisheo_backend_v2.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<bool> Exists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByFirebaseUid(string firebaseUid)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
    }

    public async Task Add(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserById(Guid userId) 
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); 
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}