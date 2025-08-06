using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;
using CorporateCMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace CorporateCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AnnouncementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

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
        public async Task<IActionResult> Create([Bind("Id,Title,Summary,Content,ImagePath,Tags,PublishDate,IsPinned,IsActive,CreatedBy")] Announcement announcement, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "announcements");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    
                    announcement.ImagePath = "/uploads/announcements/" + uniqueFileName;
                }

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
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            return View(announcement);
        }

        // POST: Admin/Announcements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,Content,ImagePath,Tags,PublishDate,IsPinned,IsActive,CreatedBy,CreatedDate,ViewCount")] Announcement announcement, IFormFile? imageFile)
        {
            if (id != announcement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(announcement.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, announcement.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "announcements");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        
                        announcement.ImagePath = "/uploads/announcements/" + uniqueFileName;
                    }

                    announcement.UpdatedDate = DateTime.UtcNow;
                    announcement.UpdatedBy = User.Identity?.Name ?? "System";

                    _context.Update(announcement);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Duyuru başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnouncementExists(announcement.Id))
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
            return View(announcement);
        }

        // GET: Admin/Announcements/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Admin/Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(announcement.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, announcement.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Duyuru başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.Id == id);
        }
    }
}
