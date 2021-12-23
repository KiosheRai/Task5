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
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Email);
                    await EditStatus("В сети", model.Email);
                    await EditTimeLogin(model.Email);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль.");
            }
            return View(model);
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
                    db.Users.Add(new User { Email = model.Email,Name = model.Name, Password = model.Password, RegisterDate = DateTime.Now, LastLoginDate = DateTime.Now, Status = "В сети" });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
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