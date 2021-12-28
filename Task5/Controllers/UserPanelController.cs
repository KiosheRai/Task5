using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.Threading.Tasks;
using Task5.Models;

namespace Task5.Controllers
{
    public class UserPanelController : Controller
    {
        private UsersContext db;

        public UserPanelController(UsersContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Email"] = User.Identity.Name;
            var x = await db.Users.ToListAsync();
            return View(x);
        }

        [HttpGet]
        public async Task<IActionResult> Send(string email)
        {
            var message = new TextMessage
            {
                Recipient = email,
            };

            return View(message);;
        }

        [HttpPost]
        public async Task<IActionResult> Send(string text, string email)
        {
            User sender = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            User recipient = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            var message = new TextMessage
            {
                Sender = sender.Email,
                Recipient = recipient.Email,
                Text = text,
                IsChecked = false,
            };

            db.Messages.Add(message);

            await db.SaveChangesAsync();

            return RedirectToAction("Index", "UserPanel");
        }
    }
}
