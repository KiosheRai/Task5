using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Email"] = User.Identity.Name;
            return View(await db.Users.ToListAsync());
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
        public async Task<IActionResult> Delete(string[] list)
        {
            bool isYouAccount = false;
            foreach (var x in list)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id.ToString() == x);
                
                if(user.Email == User.Identity.Name)
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
    }
}
