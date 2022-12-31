using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using faka.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using faka.Data;
using faka.Filters;
using faka.Models;
using faka.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace faka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly fakaContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public KeysController(fakaContext context, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            // 依赖注入
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/Keys
        [HttpGet, Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<IEnumerable<Key>>> GetKey()
        {
            return Ok(await _context.Key.ToListAsync());
        }

        // GET: api/Keys/5
        [HttpGet("{id}"), Authorize(Roles = Roles.User)]
        public async Task<ActionResult<KeyOutDto>> GetKey(int id)
        {
            var key = await _context.Key.FindAsync(id);

            if (key == null)
            {
                return NotFound();
            }
            var keyOutDto = _mapper.Map<KeyOutDto>(key);

            return Ok(keyOutDto);
        }

        // PUT: api/Keys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PutKey(int id, KeyInDto keyInDto)
        {
            var key = await _context.Key.FindAsync(id);
            if (key == null)
            {
                return NotFound();
            }
            _mapper.Map(keyInDto, key);

            _context.Entry(key).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeyExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok();
        }

        // POST: api/Keys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> PostKey(KeyBatchInDto keyBatchInDto)
        {
            var product = await _context.Product.FindAsync(keyBatchInDto.ProductId);
            if (product == null)
            {
                return BadRequest("产品不存在");
            }
            var keys = keyBatchInDto.Contents.Select(keyValue => new Key { Content = keyValue, Batch = keyBatchInDto.Batch, Product = product}).ToList();

            _context.Key.AddRange(keys);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Keys/5
        [HttpDelete("{id}"), Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> DeleteKey(int id)
        {
            var key = await _context.Key.FindAsync(id);
            if (key == null)
            {
                return NotFound();
            }

            _context.Key.Remove(key);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool KeyExists(int id)
        {
            return (_context.Key?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
