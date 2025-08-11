using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;
using CorporateCMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace CorporateCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ManageContent")]
    public class SlidersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] _allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const long _maxImageBytes = 2 * 1024 * 1024; // 2MB

        public SlidersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Sliders
        public async Task<IActionResult> Index()
        {
            var sliders = await _context.Sliders
                .OrderBy(s => s.Order)
                .ToListAsync();
            return View(sliders);
        }

        // GET: Admin/Sliders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ImagePath,Link,ButtonText,Order,OpenInNewTab,IsActive")] Slider slider, IFormFile? imageFile)
        {
            if (imageFile != null)
            {
                var result = await ProcessAndSaveImageAsync(imageFile);
                if (!result.Success)
                {
                    ModelState.AddModelError("ImageFile", result.ErrorMessage ?? "Geçersiz resim");
                }
                else
                {
                    slider.ImagePath = result.RelativePath ?? string.Empty;
                }
            }

            if (ModelState.IsValid)
            {
                slider.CreatedDate = DateTime.UtcNow;
                slider.UpdatedDate = DateTime.UtcNow;

                _context.Add(slider);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Slider başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Admin/Sliders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ImagePath,Link,ButtonText,Order,OpenInNewTab,IsActive,CreatedDate")] Slider slider, IFormFile? imageFile)
        {
            if (id != slider.Id)
            {
                return NotFound();
            }

            string? oldImagePath = null;
            if (imageFile != null)
            {
                oldImagePath = slider.ImagePath;
                var result = await ProcessAndSaveImageAsync(imageFile);
                if (!result.Success)
                {
                    ModelState.AddModelError("ImageFile", result.ErrorMessage ?? "Geçersiz resim");
                }
                else
                {
                    slider.ImagePath = result.RelativePath ?? string.Empty;
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
                    slider.UpdatedDate = DateTime.UtcNow;
                    _context.Update(slider);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Slider başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Admin/Sliders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider != null)
            {
                if (!string.IsNullOrEmpty(slider.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, slider.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Slider başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return _context.Sliders.Any(e => e.Id == id);
        }

        private async Task<(bool Success, string? RelativePath, string? ErrorMessage)> ProcessAndSaveImageAsync(IFormFile file)
        {
            if (file.Length == 0) return (false, null, "Boş dosya");
            if (file.Length > _maxImageBytes) return (false, null, $"Resim boyutu 2MB'ı aşamaz (Mevcut: {file.Length / 1024} KB)");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext)) return (false, null, "İzin verilmeyen dosya uzantısı");
            if (!_allowedContentTypes.Contains(file.ContentType)) return (false, null, "İzin verilmeyen içerik türü");

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "sliders");
            Directory.CreateDirectory(uploadsFolder);
            var safeFileName = Guid.NewGuid().ToString("N") + ext;
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);
            }
            var relative = "/uploads/sliders/" + safeFileName;
            return (true, relative, null);
        }
    }
}
