using Microsoft.EntityFrameworkCore;
using wisheo_backend_v2.Models;

namespace wisheo_backend_v2.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}