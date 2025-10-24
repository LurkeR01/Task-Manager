using System.Security.Claims;
using Domain;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Tests;

public class AuthServiceTest
{
    private AuthService CreateAuthService(out AppDbContext context, out Guid userId)
    {
        userId = Guid.NewGuid();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        context = new AppDbContext(options);

        var usersRepo = new UsersRepository(context);
        var refreshRepo = new RefreshTokensRepository(context);
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "this_is_a_very_long_test_secret_key_1234567890" },
                { "Jwt:Issuer", "test_issuer" }
            })
            .Build();
        
        var httpContextAccessor = CreateHttpContextAccessor(userId);
        
        return new AuthService(usersRepo, refreshRepo, config, httpContextAccessor);
    }
    
    private static IHttpContextAccessor CreateHttpContextAccessor(Guid userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext { User = user };
        return new HttpContextAccessor { HttpContext = context };
    }

    
    [Fact]
    public async Task Register_Should_Add_New_User_When_Username_Is_Free()
    {
        var authService = CreateAuthService(out var context, out _);

        // Act
        await authService.RegisterAsync("newUser", "password123");

        // Assert
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "newUser");
        Assert.NotNull(user);
        Assert.True(BCrypt.Net.BCrypt.Verify("password123", user.PasswordHash));
    }
    
    [Fact]
    public async Task Register_Should_Throw_Exception_When_Username_Taken()
    {
        // Arrange
        var authService = CreateAuthService(out var context, out _);
        await context.Users.AddAsync(new User { Username = "taken" });
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => authService.RegisterAsync("taken", "pass"));
    }
    
    [Fact]
    public async Task Register_Should_Hash_Password()
    {
        // Arrange
        var authService = CreateAuthService(out var context, out _);
        var plainPassword = "123456";

        // Act
        await authService.RegisterAsync("secureUser", plainPassword);
        var user = await context.Users.FirstAsync();

        // Assert
        Assert.NotEqual(plainPassword, user.PasswordHash);
        Assert.True(user.PasswordHash.StartsWith("$2")); // bcrypt-хэши обычно начинаются с $2
    }

    [Fact]
    public async Task Login_Should_Throw_Exception_When_User_Not_Found()
    {
        var authService = CreateAuthService(out var context, out _);
        
        await Assert.ThrowsAsync<Exception>(() => authService.LoginAsync("notFound", "password123"));
    }
    
    [Fact]
    public async Task Login_Should_Throw_Exception_When_Incorrect_Password()
    {
        var authService = CreateAuthService(out var context, out _);
        
        await context.Users.AddAsync(new User
        {
            Username = "tester",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345")
        });
        await context.SaveChangesAsync();
        
        await Assert.ThrowsAsync<Exception>(() => authService.LoginAsync("user", "incorrectPassword"));
    }
    
    [Fact]
    public async Task Login_Should_Return_Tokens_When_LoggedIn()
    {
        var authService = CreateAuthService(out var context, out _);
        var user = new User
        {
            Username = "tester",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345")
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        var (accessToken, refreshToken) = await authService.LoginAsync("tester", "12345");
        
        Assert.False(string.IsNullOrEmpty(accessToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));
        
        var storedToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
        Assert.NotNull(storedToken);
    }
    
    [Fact]
    public async Task RefreshToken_Should_Throw_When_TokenHash_Not_In_DB()
    {
        var authService = CreateAuthService(out _, out _);
        await Assert.ThrowsAsync<Exception>(() => authService.RefreshTokenAsync("fake_hash"));
    }
    
    [Fact]
    public async Task RefreshToken_Should_Return_NewTokens_When_Expired()
    {
        var authService = CreateAuthService(out var context, out var userId);

        var user = new User { Id = userId, Username = "u1", PasswordHash = "p" };
        await context.Users.AddAsync(user);

        var expiredToken = new RefreshToken
        {
            TokenHash = "old_hash",
            Created = DateTime.UtcNow.AddDays(-10),
            Expires = DateTime.UtcNow.AddDays(-5),
            UserId = userId
        };
        await context.RefreshTokens.AddAsync(expiredToken);
        await context.SaveChangesAsync();

        var (access, refresh) = await authService.RefreshTokenAsync("old_hash");

        Assert.False(string.IsNullOrWhiteSpace(access));
        Assert.False(string.IsNullOrWhiteSpace(refresh));
        Assert.NotEqual("old_hash", refresh);
        Assert.True(await context.RefreshTokens.AnyAsync(rt => rt.TokenHash == refresh));
    }
}