using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;
using CorporateCMS.Models;
using CorporateCMS.Models.ViewModels;

namespace CorporateCMS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Admin,Editor")]
public class PagesController : Controller
{
    private readonly ApplicationDbContext _context;

    public PagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Pages
    public async Task<IActionResult> Index()
    {
        var pages = await _context.Pages
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
        return View(pages);
    }

    // GET: Admin/Pages/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var page = await _context.Pages
            .FirstOrDefaultAsync(m => m.Id == id);
        if (page == null)
        {
            return NotFound();
        }

        return View(page);
    }

    // GET: Admin/Pages/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Pages/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PageViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Generate slug if empty
            if (string.IsNullOrEmpty(model.Slug))
            {
                model.Slug = GenerateSlug(model.Title);
            }

            // Check if slug is unique
            var existingPage = await _context.Pages
                .FirstOrDefaultAsync(p => p.Slug == model.Slug);
            if (existingPage != null)
            {
                ModelState.AddModelError("Slug", "Bu URL zaten kullanılıyor.");
                return View(model);
            }

            // If this is set as homepage, remove homepage flag from other pages
            if (model.IsHomePage)
            {
                var currentHomePage = await _context.Pages
                    .FirstOrDefaultAsync(p => p.IsHomePage);
                if (currentHomePage != null)
                {
                    currentHomePage.IsHomePage = false;
                }
            }

            var page = new Page
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

            _context.Add(page);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sayfa başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Admin/Pages/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var page = await _context.Pages.FindAsync(id);
        if (page == null)
        {
            return NotFound();
        }

        var model = new PageViewModel
        {
            Id = page.Id,
            Title = page.Title,
            Slug = page.Slug,
            Content = page.Content,
            MetaDescription = page.MetaDescription,
            MetaKeywords = page.MetaKeywords,
            IsActive = page.IsActive,
            IsHomePage = page.IsHomePage
        };

        return View(model);
    }

    // POST: Admin/Pages/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PageViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var page = await _context.Pages.FindAsync(id);
                if (page == null)
                {
                    return NotFound();
                }

                // Generate slug if empty
                if (string.IsNullOrEmpty(model.Slug))
                {
                    model.Slug = GenerateSlug(model.Title);
                }

                // Check if slug is unique (excluding current page)
                var existingPage = await _context.Pages
                    .FirstOrDefaultAsync(p => p.Slug == model.Slug && p.Id != id);
                if (existingPage != null)
                {
                    ModelState.AddModelError("Slug", "Bu URL zaten kullanılıyor.");
                    return View(model);
                }

                // If this is set as homepage, remove homepage flag from other pages
                if (model.IsHomePage && !page.IsHomePage)
                {
                    var currentHomePage = await _context.Pages
                        .FirstOrDefaultAsync(p => p.IsHomePage);
                    if (currentHomePage != null)
                    {
                        currentHomePage.IsHomePage = false;
                    }
                }

                page.Title = model.Title;
                page.Slug = model.Slug;
                page.Content = model.Content;
                page.MetaDescription = model.MetaDescription;
                page.MetaKeywords = model.MetaKeywords;
                page.IsActive = model.IsActive;
                page.IsHomePage = model.IsHomePage;
                page.UpdatedBy = User.Identity?.Name;
                page.UpdatedDate = DateTime.UtcNow;

                _context.Update(page);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sayfa başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PageExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return View(model);
    }

    // GET: Admin/Pages/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var page = await _context.Pages
            .FirstOrDefaultAsync(m => m.Id == id);
        if (page == null)
        {
            return NotFound();
        }

        return View(page);
    }

    // POST: Admin/Pages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var page = await _context.Pages.FindAsync(id);
        if (page != null)
        {
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sayfa başarıyla silindi.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PageExists(int id)
    {
        return _context.Pages.Any(e => e.Id == id);
    }

    private string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title))
            return string.Empty;

        // Turkish character replacements
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
        
        // Replace Turkish characters
        foreach (var kvp in turkishChars)
        {
            slug = slug.Replace(kvp.Key.ToString(), kvp.Value);
        }
        
        // Replace spaces and special characters with dashes
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        
        return slug;
    }
}
