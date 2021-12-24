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
        public ActionResult Index(User model)
        {
            
            return View();
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
        public ActionResult Delete(User model)
        {
            if (ModelState.IsValid)
            {
                var fruits = string.Join(",", model.Id);

                // Save data to database, and redirect to Success page.

                return Content(model.Id.ToString());
            }
            return Content("Error");
        }
    }
}
