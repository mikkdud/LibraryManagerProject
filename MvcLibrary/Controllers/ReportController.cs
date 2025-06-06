using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;

namespace MvcLibrary.Controllers
{
    public class ReportController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReportController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalBorrowings = await _context.Borrowings.CountAsync();

            var activeBorrowings = await _context.Borrowings
                .Where(b => b.ReturnedAt == null)
                .CountAsync();

            var activeUsers = await _context.Borrowings
                .Where(b => b.ReturnedAt == null)
                .Select(b => b.UserId)
                .Distinct()
                .CountAsync();

            var topBooks = await _context.Borrowings
                .Include(b => b.Volume)
                .ThenInclude(v => v.Book)
                .GroupBy(b => b.Volume.Book.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            ViewData["TotalBorrowings"] = totalBorrowings;
            ViewData["ActiveBorrowings"] = activeBorrowings;
            ViewData["ActiveUsers"] = activeUsers;
            ViewData["TopBooks"] = topBooks;

            return View();
        }
    }
}
