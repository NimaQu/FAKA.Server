using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using faka.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace faka.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    
    public AuthService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    
    public async Task<JwtSecurityToken> IssueJwtTokenAsync(IdentityUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        var authSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "defaultSecret"));

        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }

    private async Task CreateRolesAsync()
    {
        var roles = Roles.GetRoles();
        foreach (var role in roles)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    public async Task PromoToAdminAsync(IdentityUser user)
    {
        await CreateRolesAsync();
        await _userManager.AddToRoleAsync(user, Roles.Admin);
        await _userManager.AddToRoleAsync(user, Roles.User);
    }
    
    public async Task PromoToUserAsync(IdentityUser user)
    {
        await CreateRolesAsync();
        await _userManager.AddToRoleAsync(user, Roles.User);
    }
}