using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;

namespace CorporateCMS.Controllers;

public class PageController : Controller
{
    private readonly ApplicationDbContext _context;

    public PageController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /{slug}
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        var page = await _context.Pages
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

        if (page == null)
        {
            return NotFound();
        }

        // Görüntülenme sayısını artır
        page.ViewCount++;
        _context.Update(page);
        await _context.SaveChangesAsync();

        return View(page);
    }
}
