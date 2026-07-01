using System.Security.Claims;
using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.DTOs.Authentication;
using Application.Common.Mappings;
using System.Security.Cryptography;

namespace Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly SigningCredentials _signingCredentials; 
        private readonly Infrastructure.Persistence.Data.ApplicationDbContext _context;
        private readonly UrlsOptions _urlsOptions;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> jwtOptions,
            IOptions<UrlsOptions> urlsOptions,
            Infrastructure.Persistence.Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtOptions  = jwtOptions.Value;
            _urlsOptions = urlsOptions.Value;
            _context     = context;

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

            var claims = new List<Claim>(7 + userClaims.Count + roles.Count) 
            {
                new(JwtRegisteredClaimNames.Sub,        user.Id),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName   ?? string.Empty),
                new(JwtRegisteredClaimNames.Email,      user.Email      ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.GivenName,  user.FirstName  ?? string.Empty),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName   ?? string.Empty),
                new("security_stamp",                   user.SecurityStamp ?? string.Empty),
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

        public async Task<AuthResponseDto> GetAuthResponseAsync(ApplicationUser user)
        {
            await _userManager.ResetAccessFailedCountAsync(user);

            var token = await CreateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var response = user.ToAuthResponseDto(roles, _urlsOptions.API);
            response.Token = token;

            return response;
        }

        public RefreshToken GenerateRefreshToken(string jwtId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            return new RefreshToken 
            {
                Token = token, 
                JwtId = jwtId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeInDays)
            };
        }

        public async Task AddRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken)
        {
            refreshToken.UserId = user.Id;
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Token == token);
        }

        public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<bool> IsLockedOutAsync(ApplicationUser user)
            => await _userManager.IsLockedOutAsync(user);

        public async Task RecordFailedAccessAsync(ApplicationUser user)
            => await _userManager.AccessFailedAsync(user);

        public async Task<bool> ConfirmEmailAsync(ApplicationUser user, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded;
        }

        public async Task<ApplicationUser?> FindUserByIdAsync(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
            => await _userManager.GenerateEmailConfirmationTokenAsync(user);

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
            => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
            => await _userManager.ResetPasswordAsync(user, token, newPassword);

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
            => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        public async Task<IdentityResult> SetPasswordAsync(ApplicationUser user, string newPassword)
            => await _userManager.AddPasswordAsync(user, newPassword);

        public async Task<bool> HasPasswordAsync(ApplicationUser user)
            => await _userManager.HasPasswordAsync(user);

        public async Task<IdentityResult> LockUserAsync(ApplicationUser user, DateTimeOffset? lockoutEnd)
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            return await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        }

        public async Task<IdentityResult> UnLockUserAsync(ApplicationUser user)
            => await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddSeconds(-1));

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
            => await _userManager.IsInRoleAsync(user, role);

        public async Task UpdateSecurityStampAsync(ApplicationUser user)
            => await _userManager.UpdateSecurityStampAsync(user);

        public async Task ResetAccessFailedCountAsync(ApplicationUser user)
            => await _userManager.ResetAccessFailedCountAsync(user);
    }
}