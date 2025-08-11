using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CorporateCMS.Models;
using CorporateCMS.Models.ViewModels;
using CorporateCMS.Data; // added
using Microsoft.EntityFrameworkCore; // added

namespace CorporateCMS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "ManageContent")]
public class PagesController : Controller
{
    private readonly ApplicationDbContext _context; // replaced service
    private readonly ILogger<PagesController> _logger;

    public PagesController(ApplicationDbContext context, ILogger<PagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Pages
    public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 20)
    {
        if (page < 1) page = 1; if (pageSize <= 0) pageSize = 20;
        IQueryable<Page> query = _context.Pages.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p => p.Title.Contains(term) || p.Content.Contains(term));
        }
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        ViewBag.Query = q;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Total = total;
        return View(items);
    }

    // GET: Admin/Pages/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var pageEntity = await _context.Pages.FindAsync(id.Value);
        if (pageEntity == null) return NotFound();
        return View(pageEntity);
    }

    // GET: Admin/Pages/Create
    public IActionResult Create() => View();

    // POST: Admin/Pages/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PageViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        if (string.IsNullOrWhiteSpace(model.Slug)) model.Slug = GenerateSlug(model.Title);
        var existing = await _context.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == model.Slug);
        if (existing != null)
        {
            ModelState.AddModelError("Slug", "Bu URL zaten kullanılıyor.");
            return View(model);
        }
        var pageEntity = new Page
        {
            Title = model.Title,
            Slug = model.Slug,
            Content = model.Content,
            MetaDescription = model.MetaDescription,
            MetaKeywords = model.MetaKeywords,
            IsActive = model.IsActive,
            IsHomePage = model.IsHomePage,
            CreatedBy = User.Identity?.Name,
            CreatedDate = DateTime.UtcNow
        };
        _context.Pages.Add(pageEntity);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Sayfa başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Pages/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var pageEntity = await _context.Pages.FindAsync(id.Value);
        if (pageEntity == null) return NotFound();
        var model = new PageViewModel
        {
            Id = pageEntity.Id,
            Title = pageEntity.Title,
            Slug = pageEntity.Slug,
            Content = pageEntity.Content,
            MetaDescription = pageEntity.MetaDescription,
            MetaKeywords = pageEntity.MetaKeywords,
            IsActive = pageEntity.IsActive,
            IsHomePage = pageEntity.IsHomePage
        };
        return View(model);
    }

    // POST: Admin/Pages/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PageViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        if (string.IsNullOrWhiteSpace(model.Slug)) model.Slug = GenerateSlug(model.Title);
        var existingSameSlug = await _context.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == model.Slug && p.Id != id);
        if (existingSameSlug != null)
        {
            ModelState.AddModelError("Slug", "Bu URL zaten kullanılıyor.");
            return View(model);
        }
        var pageEntity = await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);
        if (pageEntity == null) return NotFound();
        pageEntity.Title = model.Title;
        pageEntity.Slug = model.Slug;
        pageEntity.Content = model.Content;
        pageEntity.MetaDescription = model.MetaDescription;
        pageEntity.MetaKeywords = model.MetaKeywords;
        pageEntity.IsActive = model.IsActive;
        pageEntity.IsHomePage = model.IsHomePage;
        pageEntity.UpdatedBy = User.Identity?.Name;
        pageEntity.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Sayfa başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Pages/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var pageEntity = await _context.Pages.FindAsync(id.Value);
        if (pageEntity == null) return NotFound();
        return View(pageEntity);
    }

    // POST: Admin/Pages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pageEntity = await _context.Pages.FindAsync(id);
        if (pageEntity != null)
        {
            _context.Pages.Remove(pageEntity);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sayfa başarıyla silindi.";
        }
        else
        {
            TempData["Error"] = "Silme işlemi başarısız.";
        }
        return RedirectToAction(nameof(Index));
    }

    private string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title)) return string.Empty;
        var turkishChars = new Dictionary<char, string>
        {
            { 'ç', "c" }, { 'Ç', "C" },
            { 'ğ', "g" }, { 'Ğ', "G" },
            { 'ı', "i" }, { 'İ', "I" },
            { 'ö', "o" }, { 'Ö', "O" },
            { 'ş', "s" }, { 'Ş', "S" },
            { 'ü', "u" }, { 'Ü', "U" }
        };
        var slug = title.ToLower();
        foreach (var kvp in turkishChars)
            slug = slug.Replace(kvp.Key.ToString(), kvp.Value);
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9-]", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
