using AutoMapper;
using FAKA.Server.Auth;
using FAKA.Server.Data;
using FAKA.Server.Models;
using FAKA.Server.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAKA.Server.Controllers;

[Route("api/v1/public/[controller]")]
[ApiController]
public class AnnouncementsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AnnouncementsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/v1/public/Announcement
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnnouncementOutDto>>> GetAnnouncements()
    {
        return _mapper.Map<List<AnnouncementOutDto>>(await _context.Announcements.ToListAsync());
    }

    // GET: api/v1/public/Announcement/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AnnouncementOutDto>> GetAnnouncement(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);

        return _mapper.Map<AnnouncementOutDto>(announcement);
    }
}