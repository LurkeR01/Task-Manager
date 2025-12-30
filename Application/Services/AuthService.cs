using System.Reflection.Emit;
using System.Security.Claims;
using Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AuthService(
        IUsersRepository usersRepository,
        IRefreshTokensRepository refreshTokensRepository,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor)
    {
        _usersRepository = usersRepository;
        _refreshTokensRepository = refreshTokensRepository;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task RegisterAsync(string username, string email, string password)
    {
        if (await _usersRepository.GetByEmailAsync(email) != null)
            throw new Exception("It seems like you have already registered with this email");

        User newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };

        await _usersRepository.AddAsync(newUser);
        await _usersRepository.SaveChangesAsync();
    }


    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password)
    {
        if (email == string.Empty || password == string.Empty)
            throw new Exception("Fill in email and password");

        var user = await _usersRepository.GetByEmailAsync(email);
        if (user == null)
            throw new Exception("User with this email is not found");

        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) == false)
            throw new Exception("Password is incorrect");

        string accessToken = GenerateAccessToken(user);
        string refresh = GenerateRefreshToken();
        string refreshTokenHash = ComputeHash(refresh);

        var refreshToken = new RefreshToken
        {
            TokenHash = refreshTokenHash,
            Created = DateTime.UtcNow,
            UserId = user.Id,
            Revoked = null,
            Expires = DateTime.UtcNow.AddDays(7),
        };

        await _refreshTokensRepository.AddAsync(refreshToken);
        await _refreshTokensRepository.SaveChangesAsync();

        return (accessToken, refreshTokenHash);
    }


    public async Task LogoutAsync(string refreshTokenHash)
    {
        var refreshToken = await _refreshTokensRepository.GetByHashAsync(refreshTokenHash);

        if (refreshToken == null)
            throw new Exception("Refresh token not found");

        refreshToken.Revoked = DateTime.UtcNow;
        await _refreshTokensRepository.SaveChangesAsync();
    }


    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshTokenHash)
    {
        var refreshToken = await _refreshTokensRepository.GetByHashAsync(refreshTokenHash);
        if (refreshToken == null)
            throw new Exception("Refresh token not found");

        var user = await _usersRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null)
            throw new Exception("User not found");

        if (refreshToken.Revoked != null)
            throw new Exception("Refresh token is revoked");

        if (refreshToken.Expires < DateTime.UtcNow)
            throw new Exception("Refresh token is expired");

        string newAccessToken = GenerateAccessToken(user);


        await _refreshTokensRepository.RemoveAsync(refreshToken);

        string newRefresh = GenerateRefreshToken();
        string newRefreshTokenHash = ComputeHash(newRefresh);

        var newRefreshToken = new RefreshToken
        {
            TokenHash = newRefreshTokenHash,
            Created = DateTime.UtcNow,
            UserId = refreshToken.UserId,
            Revoked = null,
            Expires = DateTime.UtcNow.AddDays(7),
        };

        await _refreshTokensRepository.AddAsync(newRefreshToken);
        await _refreshTokensRepository.SaveChangesAsync();

        return (newAccessToken, newRefreshTokenHash);
    }


    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    
    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
    
    
    private static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}