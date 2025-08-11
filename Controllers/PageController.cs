using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;
using CorporateCMS.Models;

namespace CorporateCMS.Controllers;

public class PageController : Controller
{
    private readonly ApplicationDbContext _context;

    public PageController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Page
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? sortBy = null)
    {
        var query = _context.Pages
            .AsNoTracking() // Performans optimizasyonu
            .Where(p => p.IsActive)
            .AsQueryable();

        // Arama işlemi
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower(); // Case-insensitive arama için
            query = query.Where(p => p.Title.ToLower().Contains(search) || 
                                    p.Content.ToLower().Contains(search) ||
                                    (p.MetaDescription != null && p.MetaDescription.ToLower().Contains(search)));
        }

        // Sıralama işlemi
        switch (sortBy)
        {
            case "title":
                query = query.OrderBy(p => p.Title);
                break;
            case "views":
                query = query.OrderByDescending(p => p.ViewCount);
                break;
            case "oldest":
                query = query.OrderBy(p => p.CreatedDate);
                break;
            default:
                query = query.OrderByDescending(p => p.CreatedDate); // Default sıralama
                break;
        }

        // Toplam kayıt sayısı
        var totalItems = await query.CountAsync();
        
        // Sayfalama
        var pages = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // ViewBag ile sayfalama bilgilerini gönder
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewBag.SearchTerm = search;
        ViewBag.SortBy = sortBy;
        ViewBag.TotalCount = totalItems;
        ViewBag.HasPreviousPage = page > 1;
        ViewBag.HasNextPage = page < ViewBag.TotalPages;

        return View(pages);
    }

    // GET: /{slug} or Page/Details/5
    public async Task<IActionResult> Details(string? slug, int? id)
    {
        if (string.IsNullOrEmpty(slug) && !id.HasValue)
        {
            return NotFound();
        }

        Page? page = null;

        if (!string.IsNullOrEmpty(slug))
        {
            page = await _context.Pages
                .AsNoTracking() // Performans optimizasyonu
                .Select(p => new Page { 
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Slug = p.Slug,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    ViewCount = p.ViewCount,
                    IsActive = p.IsActive,
                    IsHomePage = p.IsHomePage,
                    MetaDescription = p.MetaDescription,
                    MetaKeywords = p.MetaKeywords,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy
                })
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
        }
        else if (id.HasValue)
        {
            page = await _context.Pages
                .AsNoTracking() // Performans optimizasyonu
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        if (page == null)
        {
            // 404 sayfası yerine daha kullanıcı dostu bir mesaj göster
            TempData["ErrorMessage"] = "Aradığınız sayfa bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        // Görüntülenme sayısını artır (no-tracking kullandığımız için ayrı bir update sorgusu)
        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE Pages SET ViewCount = ViewCount + 1 WHERE Id = {0}", 
            page.Id
        );

        return View(page);
    }
}
