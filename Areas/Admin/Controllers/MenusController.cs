using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorporateCMS.Data;
using CorporateCMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace CorporateCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MenusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Menus
        public async Task<IActionResult> Index()
        {
            var menus = await _context.Menus
                .Include(m => m.Parent)
                .OrderBy(m => m.ParentId)
                .ThenBy(m => m.Order)
                .ToListAsync();
            return View(menus);
        }

        // GET: Admin/Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.Children)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Admin/Menus/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ParentMenus = await _context.Menus
                .Where(m => m.ParentId == null)
                .OrderBy(m => m.Order)
                .ToListAsync();
            return View();
        }

        // POST: Admin/Menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Url,Order,ParentId,IsActive,IsExternal,CssClass,Icon")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                menu.CreatedDate = DateTime.UtcNow;

                _context.Add(menu);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Menü başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentMenus = await _context.Menus
                .Where(m => m.ParentId == null)
                .OrderBy(m => m.Order)
                .ToListAsync();
            
            return View(menu);
        }

        // GET: Admin/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewBag.ParentMenus = await _context.Menus
                .Where(m => m.ParentId == null && m.Id != id)
                .OrderBy(m => m.Order)
                .ToListAsync();

            return View(menu);
        }

        // POST: Admin/Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Url,Order,ParentId,IsActive,IsExternal,CssClass,Icon,CreatedDate")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Menü başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
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

            ViewBag.ParentMenus = await _context.Menus
                .Where(m => m.ParentId == null && m.Id != id)
                .OrderBy(m => m.Order)
                .ToListAsync();

            return View(menu);
        }

        // GET: Admin/Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.Children)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Admin/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus
                .Include(m => m.Children)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menu != null)
            {
                // Check if menu has children
                if (menu.Children.Any())
                {
                    TempData["ErrorMessage"] = "Alt menüleri olan bir menü silinemez. Önce alt menüleri silin.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Menü başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
