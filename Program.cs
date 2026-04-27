using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using wisheo_backend_v2.Helpers;
using wisheo_backend_v2.Hubs;
using wisheo_backend_v2.Repositories;
using wisheo_backend_v2.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    GoogleCredential credential;
    var serviceAccountPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase-service-account.json");

    if (File.Exists(serviceAccountPath))
    {
        using var stream = File.OpenRead(serviceAccountPath);
        credential = GoogleCredential.FromStream(stream);
        Console.WriteLine("[Firebase] Initialized with service account file.");
    }
    else
    {
        credential = GoogleCredential.GetApplicationDefault();
        Console.WriteLine("[Firebase] Initialized with Application Default Credentials.");
    }

    FirebaseApp.Create(new AppOptions { Credential = credential });
}
catch (Exception ex)
{
    Console.WriteLine($"[Firebase] Could not initialize: {ex.Message}. SSO will not work.");
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<WishlistRepository>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<DeviceTokenRepository>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<FeedRepository>();
builder.Services.AddScoped<FeedService>();
builder.Services.AddScoped<SocialRepository>();
builder.Services.AddScoped<SocialService>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ProfileService>();

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not found");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization(); 
app.MapHub<SocialHub>("/hubs/social");

app.MapControllers();
app.Run();

