using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Data;
using FAKA.Server.Models;
using FAKA.Server.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnnouncementsController : ControllerBase
{
    private readonly FakaContext _context;
    private readonly IMapper _mapper;

    public AnnouncementsController(FakaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Announcement
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnnouncementOutDto>>> GetAnnouncements()
    {
        return _mapper.Map<List<AnnouncementOutDto>>(await _context.Announcements.ToListAsync());
    }

    // GET: api/Announcement/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AnnouncementOutDto>> GetAnnouncement(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);

        return _mapper.Map<AnnouncementOutDto>(announcement);
    }

    // PUT: api/Announcement/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> PutAnnouncement(int id, AnnouncementInDto announcementInDto)
    {
        var announcement = _mapper.Map<Announcement>(announcementInDto);
        _context.Entry(announcement).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnnouncementExists(id))
                return NotFound("公告不存在");
            throw;
        }

        return Ok();
    }

    // POST: api/Announcement
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> PostAnnouncement(AnnouncementInDto announcementInDto)
    {
        var announcement = _mapper.Map<Announcement>(announcementInDto);
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/Announcement/5
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private bool AnnouncementExists(int id)
    {
        return (_context.Announcements?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}