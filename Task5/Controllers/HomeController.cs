using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Task5.Models;

namespace Task5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UsersContext db;

        public HomeController(ILogger<HomeController> logger, UsersContext context)
        {
            db = context;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Email"] = User.Identity.Name;
            var x = await db.Users
                .Include(u => u.Role)
                .ToListAsync();
            return View(x);
        }

        [HttpPost]
        public ActionResult Index(string[] list)
        {
            return Content(list.ToString());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string[] list, string action) 
        {
            if(action == "Delete")
            {
                return await Delete(list);
            }
            else if(action == "Block")
            {
                return await Block(list);
            }
            else if(action == "SetAdminRole") 
            {
                return await SetAdminRole(list);
            }
            else if(action == "SetUserRole")
            {
                return await SetUserRole(list);
            }
            else
            {
                return await UnBlock(list);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string[] list)
        {
            bool isYouAccount = false;
            foreach (var x in list)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id.ToString() == x);

                if (user.Email == User.Identity.Name)
                {
                    isYouAccount = true;
                }
                db.Users.Remove(user);
                db.SaveChanges();
            }

            if (isYouAccount)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Block(string[] list)
        {
            bool isYouAccount = false;
            foreach (var x in list)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id.ToString() == x);

                if (user.Email == User.Identity.Name)
                {
                    isYouAccount = true;
                }
                user.Status = "Заблокирован";
                db.SaveChanges();
            }

            if (isYouAccount)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnBlock(string[] list)
        {
            foreach (var x in list)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id.ToString() == x);
                if (user.Email != User.Identity.Name)
                {
                    user.Status = "Нет в сети";
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SetAdminRole(string[] list)
        {
            foreach (var x in list)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id.ToString() == x);
                Role adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
                if (adminRole != null)
                    user.Role = adminRole;

                user.Role = adminRole;

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SetUserRole(string[] list)
        {
            bool isYouAccount = false;
            foreach (var x in list)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id.ToString() == x);
                Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                if (userRole != null)
                    user.Role = userRole;

                if (user.Email == User.Identity.Name)
                {
                    isYouAccount = true;
                }

                user.Role = userRole;

                db.SaveChanges();
            }

            if (isYouAccount)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Index");
        }
    }
}
