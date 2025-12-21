namespace wisheo_backend_v2.Data;

using wisheo_backend_v2.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Exists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task Add(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}