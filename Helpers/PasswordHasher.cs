using Microsoft.AspNetCore.Identity;
using wisheo_backend_v2.Models;

namespace wisheo_backend_v2.Helpers;

public static class PasswordHasher
{
    private static readonly PasswordHasher<User> _hasher = new();

    public static string Hash(User user, string password) 
        => _hasher.HashPassword(user, password);

    public static bool Verify(User user, string hashedPassword, string providedPassword) 
        => _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword) 
           == PasswordVerificationResult.Success;
}