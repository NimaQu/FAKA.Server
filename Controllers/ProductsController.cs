using AutoMapper;
using faka.Auth;
using faka.Data;
using faka.Models;
using faka.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly fakaContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public ProductsController(fakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            // 依赖注入
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOutDto>>> GetProduct()
        {
            //only get the products that are not disabled and hidden
            var products = await _context.Product.Where(p => p.IsEnabled == true && p.IsHidden == false).ToListAsync();
            var productDtos = _mapper.Map<List<ProductOutDto>>(products);
            return Ok(productDtos);
        }

        // GET: api/Products/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductOutDto>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound("产品不存在");
            }
            var productOutDto = _mapper.Map<ProductOutDto>(product);

            return Ok(productOutDto);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}"), Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PutProduct(int id, ProductInDto productInDto)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound("产品不存在");
            }
            _mapper.Map(productInDto, product);

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PostProduct(ProductInDto productInDto)
        {
            var product = _mapper.Map<Product>(productInDto);
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id:int}"), Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
