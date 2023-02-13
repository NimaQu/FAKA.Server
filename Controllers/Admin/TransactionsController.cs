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
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        // 依赖注入
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: api/v1/admin/Transactions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransaction()
    {
        if (_context.Transaction == null) return NotFound();

        return await _context.Transaction.ToListAsync();
    }

    // GET: api/v1/admin/Transactions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id)
    {
        if (_context.Transaction == null) return NotFound();

        var transaction = await _context.Transaction.FindAsync(id);

        if (transaction == null) return NotFound();

        return transaction;
    }

    // PUT: api/v1/admin/Transactions/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
    {
        if (id != transaction.Id) return BadRequest();

        _context.Entry(transaction).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TransactionExists(id)) return NotFound();
            throw;
        }

        return Ok();
    }

    // POST: api/v1/admin/Transactions
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Transaction>> PostTransaction(TransactionInDto transactionInDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionInDto);

        if (transaction.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(transaction.UserId);
            if (user == null) return BadRequest("用户不存在");
        }

        if (transaction.OrderId != null)
        {
            var order = await _context.Order.FindAsync(transaction.OrderId);
            if (order == null) return BadRequest("订单不存在");
        }

        _context.Transaction.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/v1/admin/Transactions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        if (_context.Transaction == null) return NotFound();

        var transaction = await _context.Transaction.FindAsync(id);
        if (transaction == null) return NotFound();

        _context.Transaction.Remove(transaction);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private bool TransactionExists(int id)
    {
        return (_context.Transaction?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}