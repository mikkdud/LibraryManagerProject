using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using MvcLibrary.Data;
using MvcLibrary.Models;

namespace MvcLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            return _context.Books != null ?
                        View(await _context.Books.ToListAsync()) :
                        Problem("Entity set 'LibraryDbContext.Books'  is null.");
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
                return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Description")] Book book)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (id == null || _context.Books == null)
                return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description")] Book book)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (id == null || _context.Books == null)
                return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            if (_context.Books == null)
                return Problem("Entity set 'LibraryDbContext.Books'  is null.");

            var book = await _context.Books.FindAsync(id);
            if (book != null)
                _context.Books.Remove(book);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "True";
        }
    }
}
