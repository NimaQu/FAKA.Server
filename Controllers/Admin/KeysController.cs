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
public class KeysController : ControllerBase
{
    private readonly FakaContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public KeysController(FakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/v1/admin/Keys
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Key>>> GetKey()
    {
        return Ok(await _context.Key.ToListAsync());
    }

    // GET: api/v1/admin/Keys/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Key>> GetKey(int id)
    {
        var key = await _context.Key.FindAsync(id);

        if (key == null) return NotFound();

        return Ok(key);
    }

    // PUT: api/v1/admin/Keys/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult> PutKey(int id, KeyInDto keyInDto)
    {
        var key = await _context.Key.FindAsync(id);
        if (key == null) return NotFound();
        _mapper.Map(keyInDto, key);

        _context.Entry(key).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!KeyExists(id)) return NotFound();
            throw;
        }

        return Ok();
    }

    // POST: api/v1/admin/Keys
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult> PostKey(KeyBatchInDto keyBatchInDto)
    {
        var product = await _context.Product.FindAsync(keyBatchInDto.ProductId);
        if (product == null) return BadRequest("产品不存在");
        var keys = keyBatchInDto.Contents
            .Select(keyValue => new Key { Content = keyValue, Batch = keyBatchInDto.Batch, Product = product })
            .ToList();

        _context.Key.AddRange(keys);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/v1/admin/Keys/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteKey(int id)
    {
        var key = await _context.Key.FindAsync(id);
        if (key == null) return NotFound();

        _context.Key.Remove(key);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private bool KeyExists(int id)
    {
        return (_context.Key?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}