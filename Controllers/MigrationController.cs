using AutoMapper;
using faka.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace faka.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly fakaContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public MigrationController(fakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Migration()
    {
        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any()) return Ok(pendingMigrations);

        return NotFound("No pending migrations");
    }

    [HttpPost("run")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RunMigration()
    {
        await _context.Database.MigrateAsync();
        return Ok("Yeh, You are now on .NET ASP Core");
    }
}