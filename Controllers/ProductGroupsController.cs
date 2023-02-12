using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Data;
using FAKA.Server.Models;
using FAKA.Server.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductGroupsController : ControllerBase
{
    private readonly FakaContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public ProductGroupsController(FakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/ProductGroup
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductGroupOutDto>>> GetProductGroup()
    {
        var productGroups = await _context.ProductGroup
            .Include(c => c.Products.Where(p => p.IsEnabled == true && p.IsHidden == false)).ToListAsync();
        var productGroupOutDtos = _mapper.Map<List<ProductGroupOutDto>>(productGroups);
        return productGroupOutDtos;
    }

    // GET: api/ProductGroup/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductGroup>> GetProductGroup(int id)
    {
        if (_context.ProductGroup == null) return NotFound();

        var productGroup = await _context.ProductGroup.FindAsync(id);

        if (productGroup == null) return NotFound();

        return productGroup;
    }

    // PUT: api/ProductGroup/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> PutProductGroup(int id, ProductGroupInDto productGroupInDto)
    {
        var productGroup = _mapper.Map<ProductGroup>(productGroupInDto);

        _context.Entry(productGroup).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductGroupExists(id)) return NotFound();
            throw;
        }

        return Ok();
    }

    // POST: api/ProductGroup
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> PostProductGroup(ProductGroupInDto productGroupInDto)
    {
        var productGroup = _mapper.Map<ProductGroup>(productGroupInDto);

        _context.ProductGroup.Add(productGroup);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/ProductGroup/5
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteProductGroup(int id)
    {
        if (_context.ProductGroup == null) return NotFound();

        var productGroup = await _context.ProductGroup.FindAsync(id);
        if (productGroup == null) return NotFound();

        _context.ProductGroup.Remove(productGroup);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private bool ProductGroupExists(int id)
    {
        return (_context.ProductGroup?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}