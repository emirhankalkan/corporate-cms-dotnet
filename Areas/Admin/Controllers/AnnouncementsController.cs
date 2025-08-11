using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;
using CorporateCMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions; // for slug

namespace CorporateCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ManageContent")]
    public class AnnouncementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private static readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] _allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const long _maxImageBytes = 2 * 1024 * 1024; // 2 MB

        public AnnouncementsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Announcements
        public async Task<IActionResult> Index()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.IsPinned)
                .ThenByDescending(a => a.CreatedDate)
                .ToListAsync();
            return View(announcements);
        }

        // GET: Admin/Announcements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (announcement == null)
            {
                return NotFound();
            }

            return View(announcement);
        }

        // GET: Admin/Announcements/Create
        public IActionResult Create()
        {
            var announcement = new Announcement
            {
                PublishDate = DateTime.Now
            };
            return View(announcement);
        }

        // POST: Admin/Announcements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Summary,Content,ImagePath,Tags,PublishDate,IsPinned,IsActive,CreatedBy,Slug")] Announcement announcement, IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(announcement.Slug))
                announcement.Slug = GenerateSlug(announcement.Title);
            else
                announcement.Slug = GenerateSlug(announcement.Slug);

            if (await _context.Announcements.AnyAsync(a => a.Slug == announcement.Slug))
                ModelState.AddModelError("Slug", "Bu slug zaten kullanılıyor.");

            if (imageFile != null)
            {
                var pathResult = await ProcessAndSaveImageAsync(imageFile);
                if (!pathResult.Success)
                {
                    ModelState.AddModelError("ImageFile", pathResult.ErrorMessage ?? "Geçersiz resim");
                }
                else
                {
                    announcement.ImagePath = pathResult.RelativePath;
                }
            }

            if (ModelState.IsValid)
            {
                announcement.CreatedDate = DateTime.UtcNow;
                announcement.UpdatedDate = DateTime.UtcNow;
                announcement.CreatedBy = User.Identity?.Name ?? "System";
                _context.Add(announcement);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Duyuru başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // GET: Admin/Announcements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();
            return View(announcement);
        }

        // POST: Admin/Announcements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,Content,ImagePath,Tags,PublishDate,IsPinned,IsActive,CreatedBy,CreatedDate,ViewCount,Slug")] Announcement announcement, IFormFile? imageFile)
        {
            if (id != announcement.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(announcement.Slug))
                announcement.Slug = GenerateSlug(announcement.Title);
            else
                announcement.Slug = GenerateSlug(announcement.Slug);

            if (await _context.Announcements.AnyAsync(a => a.Slug == announcement.Slug && a.Id != id))
                ModelState.AddModelError("Slug", "Bu slug zaten kullanılıyor.");

            string? oldImagePath = null;
            if (imageFile != null)
            {
                oldImagePath = announcement.ImagePath; // store before overwrite
                var pathResult = await ProcessAndSaveImageAsync(imageFile);
                if (!pathResult.Success)
                {
                    ModelState.AddModelError("ImageFile", pathResult.ErrorMessage ?? "Geçersiz resim");
                }
                else
                {
                    announcement.ImagePath = pathResult.RelativePath;
                    // delete old after new saved
                    if (!string.IsNullOrEmpty(oldImagePath))
                    {
                        var oldPhysical = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPhysical)) System.IO.File.Delete(oldPhysical);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    announcement.UpdatedDate = DateTime.UtcNow;
                    announcement.UpdatedBy = User.Identity?.Name ?? "System";
                    _context.Update(announcement);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Duyuru başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnouncementExists(announcement.Id)) return NotFound(); else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }

        // GET: Admin/Announcements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var announcement = await _context.Announcements.FirstOrDefaultAsync(m => m.Id == id);
            if (announcement == null) return NotFound();
            return View(announcement);
        }

        // POST: Admin/Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                if (!string.IsNullOrEmpty(announcement.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, announcement.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Duyuru başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementExists(int id) => _context.Announcements.Any(e => e.Id == id);

        private async Task<(bool Success, string? RelativePath, string? ErrorMessage)> ProcessAndSaveImageAsync(IFormFile file)
        {
            if (file.Length == 0) return (false, null, "Boş dosya");
            if (file.Length > _maxImageBytes) return (false, null, $"Resim boyutu 2MB'ı aşamaz (Mevcut: {file.Length / 1024} KB)");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext)) return (false, null, "İzin verilmeyen dosya uzantısı");
            if (!_allowedContentTypes.Contains(file.ContentType)) return (false, null, "İzin verilmeyen içerik türü");

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "announcements");
            Directory.CreateDirectory(uploadsFolder);
            var safeFileName = Guid.NewGuid().ToString("N") + ext; // ignore original name for safety
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);
            }
            var relative = "/uploads/announcements/" + safeFileName;
            return (true, relative, null);
        }

        private string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var map = new Dictionary<char, string>
            {
                { 'ç', "c" }, { 'Ç', "c" }, { 'ğ', "g" }, { 'Ğ', "g" }, { 'ı', "i" }, { 'İ', "i" },
                { 'ö', "o" }, { 'Ö', "o" }, { 'ş', "s" }, { 'Ş', "s" }, { 'ü', "u" }, { 'Ü', "u" }
            };
            var lower = input.ToLowerInvariant();
            foreach (var kv in map) lower = lower.Replace(kv.Key.ToString(), kv.Value);
            lower = Regex.Replace(lower, @"[^a-z0-9-]+", "-");
            lower = Regex.Replace(lower, @"-+", "-");
            return lower.Trim('-');
        }
    }
}
