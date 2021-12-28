using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.Linq;
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

        [HttpGet]
        public async Task<IActionResult> ListOfMessages()
        {
            var List = await db.Messages.Where(a => a.Recipient == User.Identity.Name).ToListAsync();

            if(List != null)
            {
                return View(List);
                
            }
            ViewData["Count"] = "Список входящих сообщений пуст.";
            return View();
        }

        public async Task<IActionResult> Edit(string[] list, string action)
        {
            if (action == "Delete")
            {
                return await Delete(list);
            }
            else if (action == "Check")
            {
                return await Check(list);
            }
            else
            {
                return await UnCheck(list);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string[] list)
        {
            foreach (var x in list)
            {
                TextMessage message = await db.Messages.FirstOrDefaultAsync(p => p.id.ToString() == x);

                db.Messages.Remove(message);
                db.SaveChanges();
            }

            return RedirectToAction("ListOfMessages");
        }

        [HttpPost]
        public async Task<IActionResult> Check(string[] list)
        {
            foreach (var x in list)
            {
                TextMessage message = await db.Messages.FirstOrDefaultAsync(p => p.id.ToString() == x);

                message.IsChecked = true;

                db.SaveChanges();
            }

            return RedirectToAction("ListOfMessages");
        }

        [HttpPost]
        public async Task<IActionResult> UnCheck(string[] list)
        {
            foreach (var x in list)
            {
                TextMessage message = await db.Messages.FirstOrDefaultAsync(p => p.id.ToString() == x);

                message.IsChecked = false;

                db.SaveChanges();
            }

            return RedirectToAction("ListOfMessages");
        }
    }
}
