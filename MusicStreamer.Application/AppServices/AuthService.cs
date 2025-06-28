using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.AppServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationMinutes;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? "MusicStreamer";
            _jwtAudience = _configuration["Jwt:Audience"] ?? "MusicStreamer";
            _jwtExpirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        public async Task<string> HashPasswordAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return await Task.Run(() =>
            {
                // Gera um salt aleatório
                byte[] salt = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);

                    byte[] hashBytes = new byte[64];
                    Array.Copy(salt, 0, hashBytes, 0, 32);
                    Array.Copy(hash, 0, hashBytes, 32, 32);
                    return Convert.ToBase64String(hashBytes);
                }
            });
        }

        public async Task<bool> ValidatePasswordAsync(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            return await Task.Run(() =>
            {
                try
                {

                    byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                    if (hashBytes.Length != 64)
                        return false;

                    byte[] salt = new byte[32];
                    Array.Copy(hashBytes, 0, salt, 0, 32);

                    using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                    {
                        byte[] hash = pbkdf2.GetBytes(32);

                        for (int i = 0; i < 32; i++)
                        {
                            if (hashBytes[i + 32] != hash[i])
                                return false;
                        }

                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await Task.Run(() =>
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("userId", user.Id.ToString())
                };

                if (user.Subscription != null)
                {
                    claims.Add(new Claim("subscriptionId", user.Subscription.PlanType.ToString()));
                    claims.Add(new Claim("isSubscriptionActive", user.Subscription.IsActive.ToString()));

                    if (user.Subscription.EndDate.HasValue)
                    {
                        claims.Add(new Claim("subscriptionEndDate", user.Subscription.EndDate.Value.ToString("yyyy-MM-dd")));
                    }
                }
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                    Issuer = _jwtIssuer,
                    Audience = _jwtAudience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            });
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public int GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                var userIdClaim = principal?.FindFirst("userId")?.Value;

                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
        public async Task<string> RenewToken(User user)
        {
            var token = await GenerateJwtTokenAsync(user);
            return token;
        }
    }
}
