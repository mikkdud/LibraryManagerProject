using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public class UserController : Controller
    {
        private readonly LibraryDbContext _context;

        public UserController(LibraryDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userLogin = HttpContext.Session.GetString("UserLogin");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            var action = context.RouteData.Values["action"]?.ToString()?.ToLower();

            // Allow ChangePassword for any logged-in user
            if (action == "changepassword")
            {
                if (string.IsNullOrEmpty(userLogin))
                {
                    context.Result = RedirectToAction("Index", "Login");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(isAdmin) || isAdmin != "True")
                {
                    context.Result = RedirectToAction("Index", "Login");
                }
            }

            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View(await _context.Users.ToListAsync()) :
                        Problem("Entity set 'LibraryDbContext.Users'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Login, string Password, string FirstName, string LastName, bool IsAdmin)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "Login and Password are required.");
                return View();
            }

            string hash = ComputeSha256Hash(Password);

            var user = new User
            {
                Login = Login,
                PasswordHash = hash,
                FirstName = FirstName,
                LastName = LastName,
                IsAdmin = IsAdmin
            };

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login,FirstName,LastName,IsAdmin")] User formUser)
        {
            if (id != formUser.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var dbUser = await _context.Users.FindAsync(id);
                    if (dbUser == null) return NotFound();

                    dbUser.Login = formUser.Login;
                    dbUser.FirstName = formUser.FirstName;
                    dbUser.LastName = formUser.LastName;
                    dbUser.IsAdmin = formUser.IsAdmin;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(formUser.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(formUser);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
                return Problem("Entity set 'LibraryDbContext.Users' is null.");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
                _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var login = HttpContext.Session.GetString("UserLogin");
            if (login == null) return RedirectToAction("Index", "Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null) return NotFound();

            string oldHash = ComputeSha256Hash(OldPassword);
            if (user.PasswordHash != oldHash)
            {
                ModelState.AddModelError("", "Incorrect current password.");
                return View();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "New passwords do not match.");
                return View();
            }

            user.PasswordHash = ComputeSha256Hash(NewPassword);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
