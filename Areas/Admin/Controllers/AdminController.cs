using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CorporateCMS.Data;
using CorporateCMS.Models;

namespace CorporateCMS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "ViewAdmin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Dashboard()
    {
        return View();
    }

    // Örnek veri oluşturma (sadece development için)
    public async Task<IActionResult> CreateSampleData()
    {
        try
        {
            // Örnek sayfa ekle
            if (!_context.Pages.Any())
            {
                var samplePages = new List<Page>
                {
                    new Page
                    {
                        Title = "Hakkımızda",
                        Slug = "hakkimizda",
                        Content = "<h2>Kurumsal CMS Hakkında</h2><p>Modern ASP.NET MVC teknolojisi ile geliştirilmiş bu içerik yönetim sistemi, kurumsal web sitelerinizi kolayca yönetmenizi sağlar.</p><p>Özellikler:</p><ul><li>Kullanıcı dostu admin paneli</li><li>SEO uyumlu yapı</li><li>Responsive tasarım</li><li>Güvenli altyapı</li></ul>",
                        MetaDescription = "Kurumsal CMS hakkında bilgi edinin",
                        MetaKeywords = "cms, içerik yönetimi, asp.net mvc",
                        IsActive = true,
                        IsHomePage = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity?.Name ?? "System"
                    },
                    new Page
                    {
                        Title = "İletişim",
                        Slug = "iletisim",
                        Content = "<h2>İletişim Bilgilerimiz</h2><div class='row'><div class='col-md-6'><h4>Adres</h4><p>Atatürk Mahallesi<br>İstanbul Caddesi No: 123<br>34000 Beyoğlu/İstanbul</p></div><div class='col-md-6'><h4>İletişim</h4><p><strong>Telefon:</strong> +90 (212) 555 0123<br><strong>E-posta:</strong> info@kurumsalcms.com<br><strong>Web:</strong> www.kurumsalcms.com</p></div></div>",
                        MetaDescription = "İletişim bilgilerimize ulaşın",
                        MetaKeywords = "iletişim, adres, telefon",
                        IsActive = true,
                        IsHomePage = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity?.Name ?? "System"
                    }
                };

                _context.Pages.AddRange(samplePages);
            }

            // Örnek duyuru ekle
            if (!_context.Announcements.Any())
            {
                var sampleAnnouncements = new List<Announcement>
                {
                    new Announcement
                    {
                        Title = "Kurumsal CMS Tanıtımı",
                        Summary = "Yeni içerik yönetim sistemimiz hizmetinizde!",
                        Content = "<p>Modern ASP.NET MVC teknolojisi ile geliştirilen Kurumsal CMS artık kullanıma hazır.</p>",
                        IsActive = true,
                        IsPinned = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity?.Name ?? "System"
                    },
                    new Announcement
                    {
                        Title = "Sistem Güncellemesi",
                        Summary = "Güvenlik ve performans iyileştirmeleri yapıldı.",
                        Content = "<p>Sistemimiz en son güvenlik güncellemeleri ile güçlendirildi.</p>",
                        IsActive = true,
                        IsPinned = false,
                        CreatedDate = DateTime.Now.AddDays(-1),
                        CreatedBy = User.Identity?.Name ?? "System"
                    }
                };

                _context.Announcements.AddRange(sampleAnnouncements);
            }

            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Örnek veriler başarıyla oluşturuldu!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Örnek veri oluşturulurken hata: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}
