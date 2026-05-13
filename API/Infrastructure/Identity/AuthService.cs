using System.Security.Claims;
using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly SigningCredentials _signingCredentials; 

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions  = jwtOptions.Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
            => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<bool> IsUserNameExistsAsync(string userName)
            => await _userManager.FindByNameAsync(userName) is not null;

        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var now = DateTime.UtcNow; 

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles      = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>(6 + userClaims.Count + roles.Count) 
            {
                new(JwtRegisteredClaimNames.Sub,        user.Id),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName   ?? string.Empty),
                new(JwtRegisteredClaimNames.Email,      user.Email      ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.GivenName,  user.FirstName  ?? string.Empty),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName   ?? string.Empty),
            };

            claims.AddRange(userClaims);
            claims.AddRange(roles.Select(r => new Claim("roles", r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject             = new ClaimsIdentity(claims),
                NotBefore           = now,                                          
                IssuedAt            = now,                                         
                Expires             = now.AddMinutes(_jwtOptions.LifetimeInMinutes),
                SigningCredentials  = _signingCredentials,                        
                Issuer              = _jwtOptions.Issuer,
                Audience            = _jwtOptions.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}