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
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/v1/public/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductOutDto>>> GetProduct()
    {
        //only get the products that are not disabled and hidden
        var products = await _context.Product.Where(p => p.IsEnabled == true && p.IsHidden == false).ToListAsync();
        var productDtos = _mapper.Map<List<ProductOutDto>>(products);
        return Ok(productDtos);
    }

    // GET: api/v1/public/Products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductOutDto>> GetProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);

        if (product == null) return NotFound("产品不存在");
        var productOutDto = _mapper.Map<ProductOutDto>(product);

        return Ok(productOutDto);
    }
}