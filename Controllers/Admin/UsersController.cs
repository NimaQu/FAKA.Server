using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers.Admin;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMapper _mapper;
    
    public UsersController(UserManager<IdentityUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    
    // GET: api/v1/admin/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserOutDto>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userOutDtos = _mapper.Map<List<UserOutDto>>(users);
        return userOutDtos;
    }
    
    // GET: api/v1/admin/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserOutDto>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        
        if (user == null) return NotFound();
        var userOutDtos = _mapper.Map<UserOutDto>(user);
        
        return userOutDtos;
    }
    
    // PUT: api/v1/admin/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(string id, UserInDto userInDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        _mapper.Map(userInDto, user);
        
        if (userInDto.NewPassword != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, userInDto.NewPassword);
        }
        
        try
        {
            await _userManager.UpdateAsync(user);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        
        return NoContent();
    }
    
    // DELETE: api/v1/admin/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        
        await _userManager.DeleteAsync(user);
        
        return NoContent();
    }
    
    private bool UserExists(string id)
    {
        return _userManager.Users.Any(e => e.Id == id);
    }
}