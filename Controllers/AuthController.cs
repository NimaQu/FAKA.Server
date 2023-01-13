using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using faka.Auth;
using faka.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace faka.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AuthService _authService;

    public AuthController(UserManager<IdentityUser> userManager, AuthService authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model.Username == null || model.Password == null) return BadRequest("Username or password is null");
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Invalid username or password");

        var token = await _authService.IssueJwtTokenAsync(user);
        //add token to header
        Response.Headers.Add("Authorization", $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}");

        return Ok();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (model.Username == null || model.Password == null) return BadRequest("Username or password is null");
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return BadRequest("User already exists!");

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        //发送验证邮件
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var token = await _authService.IssueJwtTokenAsync(user);
        //add token to header
        Response.Headers.Add("Authorization", $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}");
        // dev only
        return Ok(code);
    }

    [HttpPost]
    [Route("register-admin")]
    // dev only
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        if (model.Username == null || model.Password == null) return BadRequest("Username or password is null");
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Account already exists!");

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, "User create failed");
        await _authService.PromoToAdminAsync(user);
        return Ok("Account created successfully!");
    }

    [HttpPost]
    [Authorize]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string code)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized("请先登录");
        var result = await _userManager.ConfirmEmailAsync(user, code);
        Console.WriteLine(result.Errors);
        if (!result.Succeeded) return BadRequest("链接无效或已过期");
        await _authService.PromoToUserAsync(user);
        // 因为是邮箱验证，所以直接登录并且更新token，因为未确认邮箱没有 User 这个 Role
        var token = await _authService.IssueJwtTokenAsync(user);
        //add token to header
        Response.Headers.Add("Authorization", $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}");
        return Ok("邮箱验证成功");
    }
}