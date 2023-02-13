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

[Route("api/v1/public/[controller]")]
[ApiController]
public class ProductGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductGroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/v1/public/ProductGroup
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductGroupOutDto>>> GetProductGroup()
    {
        var productGroups = await _context.ProductGroup
            .Include(c => c.Products.Where(p => p.IsEnabled == true && p.IsHidden == false)).ToListAsync();
        var productGroupOutDtos = _mapper.Map<List<ProductGroupOutDto>>(productGroups);
        return productGroupOutDtos;
    }

    // GET: api/v1/public/ProductGroup/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductGroupOutDto>> GetProductGroup(int id)
    {
        if (_context.ProductGroup == null) return NotFound();

        var productGroup = await _context.ProductGroup.FindAsync(id);

        if (productGroup == null) return NotFound();
        var productGroupOutDtos = _mapper.Map<ProductGroupOutDto>(productGroup);

        return productGroupOutDtos;
    }
}