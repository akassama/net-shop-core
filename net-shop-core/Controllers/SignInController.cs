using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using net_shop_core.Models;

namespace ModestLiving.Controllers
{
    public class SignInController : Controller
    {
        AppFunctions functions = new AppFunctions();

        private readonly DBConnection _context;
        private readonly SessionManager _sessionManager;

        public SignInController(SessionManager sessionManager, DBConnection context)
        {
            _sessionManager = sessionManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                // check a password
                var query = _context.Accounts.Where(s => s.Email == loginModel.Email);
                string hashedPassword = (query.Any()) ? query.FirstOrDefault().Password : "";
                if (!string.IsNullOrEmpty(hashedPassword))
                {
                    if (BCrypt.Net.BCrypt.Verify(loginModel.Password, hashedPassword))
                    {
                        TempData["Message"] = "Login Success";

                        //Set session
                        _sessionManager.ID = query.FirstOrDefault().ID;
                        _sessionManager.LoginName = query.FirstOrDefault().Email.Split('@')[0];
                        _sessionManager.LoginEmail = query.FirstOrDefault().Email;

                        //return RedirectToAction("Index", "Account");
                        return Content("You are logged in!");
                    }
                }
                TempData["Message"] = "Wrong login details";
                return View(loginModel);
            }
            TempData["Message"] = "Login Failed";
            return View(loginModel);
        }
    }
}