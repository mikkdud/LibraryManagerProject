using Microsoft.AspNetCore.Mvc;
using MvcLibrary.Data;
using MvcLibrary.Models;
using System.Security.Cryptography;
using System.Text;

namespace MvcLibrary.Controllers
{
    public class LoginController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(LibraryDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Index(string login, string password)
        {
            string hash = ComputeSha256Hash(password);
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == hash);
            if (user != null)
            {
                HttpContext.Session.SetString("UserLogin", user.Login);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid login or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
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
