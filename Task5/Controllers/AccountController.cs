using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task5.ViewModels;
using Task5.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private UsersContext db;
        public AccountController(UsersContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null && user.Status != "Заблокирован")
                {
                    await Authenticate(user);
                    await EditStatus("В сети", model.Email);
                    await EditTimeLogin(model.Email);

                    return await RederectToRole(user);
                }
                if (user != null && user.Status == "Заблокирован")
                {
                    ModelState.AddModelError("", "Пользователь заблокирован");
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> RederectToRole(User user)
        {
            if(user.Role.Name == "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "UserPanel");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new User { Email = model.Email,Name = model.Name, Password = model.Password, RegisterDate = DateTime.Now, LastLoginDate = DateTime.Now, Status = "Не в сети" };
                    Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    db.Users.Add(user);

                    await db.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await EditStatus("Не в сети", User.Identity.Name);
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> EditStatus(string s, string Email) //Repetition
        {
            if (Email != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Email == Email);
                if (user != null)
                {
                    user.Status = s; 
                    await db.SaveChangesAsync();
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> EditTimeLogin(string Email) //Repetition
        {
            if (Email != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Email == Email);
                if (user != null)
                {
                    user.LastLoginDate = DateTime.Now;
                    await db.SaveChangesAsync();
                }
            }
            return NotFound();
        }
    }
}