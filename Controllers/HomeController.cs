using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Models;
using CorporateCMS.Data;

namespace CorporateCMS.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Ana sayfa olarak işaretlenmiş sayfa varsa onu göster
        var homePage = await _context.Pages
            .FirstOrDefaultAsync(p => p.IsHomePage && p.IsActive);

        if (homePage != null)
        {
            return View("HomePage", homePage);
        }

        // Ana sayfa yoksa varsayılan ana sayfa şablonunu göster
        ViewBag.Sliders = await _context.Sliders
            .Where(s => s.IsActive)
            .OrderBy(s => s.Order)
            .ToListAsync();

        ViewBag.RecentPages = await _context.Pages
            .Where(p => p.IsActive && !p.IsHomePage)
            .OrderByDescending(p => p.CreatedDate)
            .Take(6)
            .ToListAsync();

        ViewBag.Announcements = await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.CreatedDate)
            .Take(5)
            .ToListAsync();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
