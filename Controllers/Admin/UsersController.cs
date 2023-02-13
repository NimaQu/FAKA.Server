using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Data;
using FAKA.Server.Models;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    
    public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _context = context;
    }
    
    // // GET: api/v1/admin/Users
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<UserOutDto>>> GetUsers()
    // {
    //     var users = await _userManager.Users.ToListAsync();
    //     var userOutDtos = _mapper.Map<List<UserOutDto>>(users);
    //     return userOutDtos;
    // }
    
    // GET: api/v1/admin/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserOutDto>>> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ToListAsync();
        var roles = await _context.Roles.ToListAsync();
        var userOutDtos = new List<UserOutDto>();
        foreach (var user in users)
        {
            
            var userOutDto = _mapper.Map<UserOutDto>(user);
            
            // var rolesList = new List<string>();
            // foreach (var userRole in user.UserRoles)
            // {
            //     var role = roles.Find(r => r.Id == userRole.RoleId);
            //     if (role?.Name == null) continue;
            //     rolesList.Add(role.Name);
            // } 用用户的角色ID在角色表中查找角色名，放入 rolesList，转换为 linq 语句如下：
            var rolesList = (from userRole in user.UserRoles select roles.Find(r => r.Id == userRole.RoleId) into role where role?.Name != null select role.Name).ToList();

            userOutDto.Roles = rolesList;
            userOutDtos.Add(userOutDto);
        }
        return userOutDtos;
    }
    
    // GET: api/v1/admin/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserOutDto>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        
        if (user == null) return NotFound();
        var userOutDtos = _mapper.Map<UserOutDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userOutDtos.Roles = new List<string>(roles);
        
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
        
        //重置密码
        if (userInDto.NewPassword != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, userInDto.NewPassword);
        }
        
        //重置角色
        if (userInDto.NewRoles != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRolesAsync(user, userInDto.NewRoles);
        }
        
        try
        {
            await _userManager.UpdateAsync(user);
        }
        catch (DbUpdateConcurrencyException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e);
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