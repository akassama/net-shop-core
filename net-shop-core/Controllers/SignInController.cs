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
                        //If account not activated
                        if(query.FirstOrDefault().Status == 0)
                        {
                            TempData["ErrorMessage"] = "Account not activated.";
                            return View(loginModel);
                        }
                        //If account suspended
                        if (query.FirstOrDefault().Status == 2)
                        {
                            TempData["ErrorMessage"] = "Account suspended.";
                            return View(loginModel);
                        }

                        //Set sessions
                        _sessionManager.ID = query.FirstOrDefault().ID;
                        _sessionManager.LoginUsername = query.FirstOrDefault().Email.Split('@')[0];
                        _sessionManager.LoginEmail = query.FirstOrDefault().Email;
                        _sessionManager.LoginFirstName =  (query.FirstOrDefault().FirstName != null) ? query.FirstOrDefault().FirstName : "";
                        _sessionManager.LoginLastName = (query.FirstOrDefault().LastName != null) ? query.FirstOrDefault().LastName : "";
                        _sessionManager.LoginDirectoryName = query.FirstOrDefault().DirectoryName;
                        if (query.FirstOrDefault().Oauth == 0)
                        {
                            //set profile pic to default if null
                            _sessionManager.LoginProfilePicture = (query.FirstOrDefault().FirstName != null) ? "/files/" + _sessionManager.LoginDirectoryName + "/profile/"+ query.FirstOrDefault().ProfilePicture : "/files/defaults/account/default.jpg";

                        }
                        else
                        {
                            _sessionManager.LoginProfilePicture = "/files/defaults/account/default.jpg";
                        }

                        return RedirectToAction("Index", "Account");
                    }
                }
                TempData["ErrorMessage"] = "Login failed. You have entered an invalid email or password";
                return View(loginModel);
            }
            TempData["ErrorMessage"] = "Login failed";
            return View(loginModel);
        }
    }
}