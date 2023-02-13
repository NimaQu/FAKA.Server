using System.IdentityModel.Tokens.Jwt;
using FAKA.Server.Auth;
using FAKA.Server.Models;
using FAKA.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FAKA.Server.Controllers;

[Route("api/v1/public/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthService _authService;
    private readonly EmailService _emailService;

    public AuthController(UserManager<ApplicationUser> userManager, AuthService authService, EmailService emailService)
    {
        _userManager = userManager;
        _authService = authService;
        _emailService = emailService;
    }
    
    // POST: api/v1/public/Auth/login
    [HttpPost("login")]
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

    // POST: api/v1/public/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (model.Username == null || model.Password == null) return BadRequest("Username or password is null");
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return BadRequest("User already exists!");

        ApplicationUser user = new()
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
        await _emailService.SendVerificationEmailAsync(user.Email, code);
        return Ok(code);
    }

    // POST: api/v1/public/Auth/register-admin
    [HttpPost("register-admin")]
    // dev only
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        if (model.Username == null || model.Password == null) return BadRequest("Username or password is null");
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Account already exists!");

        ApplicationUser user = new()
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

    // POST: api/v1/public/Auth/confirm-email
    [HttpPost("confirm-email")]
    [Authorize]
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