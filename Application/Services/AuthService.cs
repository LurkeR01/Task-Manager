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

    
    public async Task RegisterAsync(string username, string password)
    {
        if (_usersRepository.GetByUsernameAsync(username).GetAwaiter().GetResult() != null) 
            throw new Exception("Username is already taken");

        User newUser = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };
        
        await _usersRepository.AddAsync(newUser);
        await _usersRepository.SaveChangesAsync();
    }

    
    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string username, string password)
    {
        var user = await _usersRepository.GetByUsernameAsync(username);
        if (user == null)
            throw new Exception("Username not found");
        
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


    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshTokenHash)
    {
        var claimsUser = _httpContextAccessor.HttpContext?.User;
        var userId = Guid.Parse(claimsUser?.FindFirstValue(ClaimTypes.NameIdentifier));

        var user = await _usersRepository.GetByIdAsync(userId);
        var refreshToken = await _refreshTokensRepository.GetByHashAsync(refreshTokenHash);
        
        if (refreshToken == null) 
            throw new Exception("Refresh token not found");

        string newAccessToken = GenerateAccessToken(user);
        
        if (refreshToken.Revoked != null || refreshToken.Expires < DateTime.UtcNow)
        {
            await _refreshTokensRepository.RemoveAsync(refreshToken);

            string newRefresh = GenerateRefreshToken();
            string newRefreshTokenHash = ComputeHash(newRefresh);
            
            var newRefreshToken = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                Created = DateTime.UtcNow,
                UserId = userId,
                Revoked = null,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            
            await _refreshTokensRepository.AddAsync(newRefreshToken);
            await _refreshTokensRepository.SaveChangesAsync();
            
            return (newAccessToken, newRefreshTokenHash);
        }
        
        return (newAccessToken, refreshTokenHash);
    } 

    
    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("isAdmin", user.IsAdmin.ToString()),
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