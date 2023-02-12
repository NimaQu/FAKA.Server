using AutoMapper;
using FAKA.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers.Admin;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class MigrationController : ControllerBase
{
    private readonly FakaContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public MigrationController(FakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/v1/admin/Migration
    [HttpGet]
    public async Task<ActionResult> Migration()
    {
        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any()) return Ok(pendingMigrations);

        return NotFound("No pending migrations");
    }

    // POST: api/v1/admin/Migration
    [HttpPost("run")]
    public async Task<ActionResult> RunMigration()
    {
        await _context.Database.MigrateAsync();
        return Ok("Yeh, You are now on .NET ASP Core");
    }
}