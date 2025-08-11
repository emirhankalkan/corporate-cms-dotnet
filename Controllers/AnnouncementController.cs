using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;

namespace CorporateCMS.Controllers;

public class AnnouncementController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnnouncementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Announcement
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var announcements = await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = await _context.Announcements.CountAsync(a => a.IsActive);

        return View(announcements);
    }

    // GET: /Announcement/Details/5 or /Announcement/slug
    [Route("Announcement/Details/{id:int}")]
    [Route("Announcement/{slug}")]
    public async Task<IActionResult> Details(int? id, string? slug)
    {
        Models.Announcement? announcement = null;
        if (id.HasValue)
        {
            announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id.Value && a.IsActive);
        }
        else if (!string.IsNullOrWhiteSpace(slug))
        {
            announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Slug == slug && a.IsActive);
        }

        if (announcement == null)
        {
            return NotFound();
        }

        announcement.ViewCount++;
        _context.Update(announcement);
        await _context.SaveChangesAsync();

        return View(announcement);
    }
}
