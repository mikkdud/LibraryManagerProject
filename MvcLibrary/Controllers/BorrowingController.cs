using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcLibrary.Data;
using MvcLibrary.Models;

namespace MvcLibrary.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly LibraryDbContext _context;

        public BorrowingController(LibraryDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isLoggedIn = HttpContext.Session.GetString("UserLogin");
            if (string.IsNullOrEmpty(isLoggedIn))
            {
                context.Result = RedirectToAction("Index", "Login");
            }
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.User)
                .Include(b => b.Volume)
                    .ThenInclude(v => v.Book)
                .ToListAsync();

            return View(borrowings);
        }

        public IActionResult Create()
        {
            var availableVolumes = _context.Volumes
                .Include(v => v.Book)
                .Where(v => v.IsAvailable)
                .ToList();

            ViewData["VolumeId"] = new SelectList(availableVolumes, "Id", "InventoryNumber");

            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            if (isAdmin == "True")
            {
                ViewData["UserId"] = new SelectList(_context.Users.ToList(), "Id", "Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int VolumeId, DateTime BorrowedAt, int? UserId)
        {
            var login = HttpContext.Session.GetString("UserLogin");
            if (string.IsNullOrEmpty(login)) return RedirectToAction("Index", "Login");

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (currentUser == null) return NotFound();

            int targetUserId = UserId ?? currentUser.Id;

            var volume = await _context.Volumes.FindAsync(VolumeId);
            if (volume == null || !volume.IsAvailable) return BadRequest("Volume is not available.");

            var borrowing = new Borrowing
            {
                UserId = targetUserId,
                VolumeId = VolumeId,
                BorrowedAt = BorrowedAt
            };

            volume.IsAvailable = false;

            _context.Borrowings.Add(borrowing);
            _context.Volumes.Update(volume);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var borrowing = await _context.Borrowings.FindAsync(id);
            if (borrowing == null) return NotFound();

            return View(borrowing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReturnedAt")] Borrowing borrowing)
        {
            if (id != borrowing.Id) return NotFound();

            var existing = await _context.Borrowings.FindAsync(id);
            if (existing == null) return NotFound();

            existing.ReturnedAt = borrowing.ReturnedAt;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var borrowing = await _context.Borrowings
                .Include(b => b.User)
                .Include(b => b.Volume)
                    .ThenInclude(v => v.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (borrowing == null) return NotFound();

            return View(borrowing);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var borrowing = await _context.Borrowings
                .Include(b => b.User)
                .Include(b => b.Volume)
                    .ThenInclude(v => v.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (borrowing == null) return NotFound();

            return View(borrowing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Volume)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing != null)
            {
                borrowing.Volume.IsAvailable = true;
                _context.Volumes.Update(borrowing.Volume);
                _context.Borrowings.Remove(borrowing);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BorrowingExists(int id)
        {
            return (_context.Borrowings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
